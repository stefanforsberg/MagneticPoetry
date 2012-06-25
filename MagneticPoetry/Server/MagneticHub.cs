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
        // http://www.opensourceshakespeare.org/views/plays/play_view.php?WorkID=kinglear&Act=2&Scene=3&Scope=scene
        string _text = @"
        I heard myself proclaim'd, 
        And by the happy hollow of a tree 
        Escap'd the hunt. No port is free, no place 
        That guard and most unusual vigilance 1255
        Does not attend my taking. Whiles I may scape, 
        I will preserve myself; and am bethought 
        To take the basest and most poorest shape 
        That ever penury, in contempt of man, 
        Brought near to beast. My face I'll grime with filth, 1260
        Blanket my loins, elf all my hair in knots, 
        And with presented nakedness outface 
        The winds and persecutions of the sky. 
        The country gives me proof and precedent 
        Of Bedlam beggars, who, with roaring voices, 1265
        Strike in their numb'd and mortified bare arms 
        Pins, wooden pricks, nails, sprigs of rosemary; 
        And with this horrible object, from low farms, 
        Poor pelting villages, sheepcotes, and mills, 
        Sometime with lunatic bans, sometime with prayers, 1270
        Enforce their charity. 'Poor Turlygod! poor Tom!' 
        That's something yet! Edgar I nothing am. Exit";

        static ConcurrentDictionary<int, Word> _words; 

        public void Join()
        {
            if(_words == null)
            {
                
                GeneratorWords();
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

        void GeneratorWords()
        {
            var allWords = _text
                .Replace(".", string.Empty)
                .Replace(",", string.Empty)
                .Replace(";", string.Empty)
                .Split(new[] {" ", Environment.NewLine}, StringSplitOptions.None)
                .Select(w => w.Trim())
                .Where(w => w.Length > 0)
                .Take(100)
                .ToArray();

            _words = new ConcurrentDictionary<int, Word>();

            var random = new Random();

            for (int i = 0; i < allWords.Count(); i++)
            {
                _words.TryAdd(i + 1, new Word
                                         {
                                             Id = i + 1,
                                             IsDragged = false,
                                             Title = allWords[i],
                                             X = random.Next(10, 750),
                                             Y = random.Next(10, 450),
                                         });
            }
        }
    }
}