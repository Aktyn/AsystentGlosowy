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
        public static int counter = 0;
        private static int abacus;
        public static void add(string _title, string _video_id)
        {
            playlistMemory.Add(new Playlist.PlaylistSchema 
               {
					res = "request_song", 
					video_id = _video_id, 
					title = _title
                });
        }
        public static bool check()
        {
            if (abacus != counter)
                return true;
            return false;
        }
        public static void skip()
        {
                if(check())
                {
                    PlaylistSchema temp = new PlaylistSchema();
                    temp = Playlist.playlistMemory[0];
                    Playlist.playlistMemory.RemoveAt(0);		
                    Playlist.playlistMemory.Add(temp);
                    abacus++;
                }
        }
        /*
        public string find()
        {
            return;
        }
        */
    }

    public class PlayNow
    {
        public static string title;
        public static string video_id;

        public static void playNow(string _title, string _video_id)
        {
            title = _title;
            video_id = _video_id;
        }
    }
}