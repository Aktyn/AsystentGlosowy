using System;
using System.Collections.Generic;
using Asystent.common;

namespace Asystent.procedures
{
    public class Playlist
    {
        public struct PlaylistEndSchema
        {
            public string res {get; set;}
        }

        public static List<VideosEntry> playlistMemory = new List<VideosEntry>();
        public static VideosEntry current = null;

        public static bool isEmpty()
        {
            if(playlistMemory.Count == 0)
                return true;
            return false;
        }
       
        public static void add(VideosEntry videos)
        {
            playlistMemory.Add(videos);
            Console.WriteLine(" --- > Dodanie do playlity. | Ilość: " + playlistMemory.Count);

        }
        
        public static VideosEntry getNext()
        {
            VideosEntry first = playlistMemory[0];
            playlistMemory.RemoveAt(0);
            current = first;

            Console.WriteLine(" --- > Pobranie z playlisty. | Ilość: " + playlistMemory.Count);
            return first;
        }

        public static void clear() {
            current = null;
            playlistMemory.Clear();
        }
    }
}