$(function () {

    var hub = $.connection.magnetic;
    var $ma = $("#magnetic-area");

    $.extend(hub, {
        wordMoved: function (cid, id, x, y) {
            if ($.connection.hub.id !== cid) {
                var tile = $("#tile-" + id);
                tile.draggable('disable');
                tile.addClass("occupied");
                tile.css({ left: x, top: y });
            }
        },
        wordStopped: function (cid, id) {
            if ($.connection.hub.id !== cid) {
                var tile = $("#tile-" + id);
                tile.draggable('enable');
                tile.removeClass("occupied");
            }
        },
        setup: function (words) {
            var wordsHtml = "";

            for (var i = 0; i < words.length; i++) {
                wordsHtml += "<span id='tile-" + words[i].Id + "' data-id='" + words[i].Id + "' class ='word' style='left: " + words[i].X + "px; top: " + words[i].Y + "px; z-index: " + words[i].Id + "'>" + words[i].Title + "</span>";
            }

            $ma.append(wordsHtml);

            $(".word").draggable(
                        {
                            containment: "#magnetic-area",
                            scroll: false,
                            drag: function () {
                                hub.move($(this).data("id"), $(this).offset().left, $(this).offset().top);
                            },
                            stop: function () {
                                hub.stop($(this).data("id"));
                            }
                        });
        }
    });

    $.connection.hub.start().done(function () {
        hub.join();
    });
})