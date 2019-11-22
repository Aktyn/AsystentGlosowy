using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Asystent.procedures
{
    public class Playlist
    {
        public struct PlaylistEndSchema
        {
            public string res {get; set;}
        }

        public static List<VideoInfo> playlistMemory = new List<VideoInfo>();
        public static VideoInfo currentVideo = null;

        public static bool isEmpty()
        {
            if(playlistMemory.Count == 0)
                return true;
            return false;
        }
       
        public static void add(VideoInfo video)
        {
            playlistMemory.Add(video);
            Console.WriteLine(" --- > Dodanie do playlity. | Ilość: " + playlistMemory.Count);

        }
        
        public static VideoInfo getNext()
        {
            VideoInfo first = playlistMemory[0];
            playlistMemory.RemoveAt(0);
            currentVideo = first;

            Console.WriteLine(" --- > Pobranie z playlisty. | Ilość: " + playlistMemory.Count);
            return first;
        }
        /*
        public static void skip()
        {

        }
        */
        /*
        public string find()
        {
            return;
        }
        */
    }
}