using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Asystent.procedures
{
    public class Playlist
    {
        public struct PlaylistSchema
        {
            public string playlistName {get; set;}
            public string res { get; set; }
            public string video_id { get; set; }
            public string title { get; set; }
        }


        public static List<PlaylistSchema> playlistMemory = new List<PlaylistSchema>();

        public string playlistName;
        public static bool Empty = true;

        //public static int counter = 0;
        void add()
        {
            ;
        }
        public static void skip()
        {
                PlaylistSchema temp = new PlaylistSchema();
                temp = Playlist.playlistMemory[0];
                Playlist.playlistMemory.RemoveAt(0);		
                Playlist.playlistMemory.Add(temp);
        }

        /*
        public string find()
        {
            return;
        }
        */

    }



/// --------------------------------

}