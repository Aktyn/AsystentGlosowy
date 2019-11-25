using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Asystent.common;

namespace Asystent.procedures
{
    public class Playlist
    {
        public struct PlaylistEndSchema
        {
            public string res {get; set;}
        }

        public static List<VideosEntry> playlistState = new List<VideosEntry>();
        private static VideosEntry current = null;

        public static VideosEntry getCurrent() {
            return current;
        }
        public static void setCurrent(VideosEntry videos) {
            Playlist.current = videos;
            update();
        }

        public static void update() {
            ClientConnection.Instance().DistributeMessage(JsonConvert.SerializeObject(new PlaylistStateUpdate{
                res = "playlist_update",
                state = playlistState,
                current = current
            }));
        }

        public static bool isEmpty()
        {
            if(playlistState.Count == 0)
                return true;
            return false;
        }
       
        public static void add(VideosEntry videos)
        {
            playlistState.Add(videos);
            Console.WriteLine(" --- > Dodanie do playlity. | Ilość: " + playlistState.Count);
            update();
        }
        
        public static VideosEntry getNext()
        {
            VideosEntry first = playlistState[0];
            playlistState.RemoveAt(0);
            current = first;

            Console.WriteLine(" --- > Pobranie z playlisty. | Ilość: " + playlistState.Count);
            update();
            return first;
        }

        public static void clear() {
            current = null;
            playlistState.Clear();
        }
    }
}