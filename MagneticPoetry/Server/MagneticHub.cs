using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SignalR.Hubs;

namespace MagneticPoetry.Server
{
    [HubName("magnetic")]
    public class MagneticHub : Hub
    {
        static ConcurrentDictionary<int, Word> _words;

        public void Join()
        {
            if(_words == null)
            {
                GenerateWords();
            }

            Caller.setup(_words.Select(w => w.Value));
        }

        public void Move(int id, int x, int y)
        {
            var tile = _words[id];
            tile.X = x;
            tile.Y = y;
            tile.IsDragged = true;
            
            Clients.wordMoved(Context.ConnectionId, id, x, y);
        }

        public void Stop(int id)
        {
            var tile = _words[id];
            tile.IsDragged = false;

            Clients.wordStopped(Context.ConnectionId, id);
        }

        private void GenerateWords()
        {
            var allWords = new WordGenerator().GeneratorWords();
            _words = new ConcurrentDictionary<int, Word>();

            var random = new Random();

            for (var i = 0; i < allWords.Count(); i++)
            {
                _words.TryAdd(i + 1, new Word
                                         {
                                             Id = i + 1,
                                             IsDragged = false,
                                             Title = allWords[i],
                                             X = random.Next(10, 920),
                                             Y = random.Next(10, 630),
                                         });
            }
        }
    }
}