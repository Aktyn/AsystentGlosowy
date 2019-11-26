using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Asystent.common;

namespace Asystent.procedures
{
    public class Playlist
    {
        public struct PlaylistEndSchema
        {
            public string res { get; set; }
        }

        public static string directory = @"C:/Users/jarek/Desktop/Nowy folder (3)/Playlista.json";

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

        public static bool isEmpty() {
            return playlistState.Count == 0;
        }
       
        public static void add(VideosEntry videos) {
            playlistState.Add(videos);
            Console.WriteLine(" --- > Dodanie do playlity. | Ilość: " + playlistState.Count);
            update();
        }
        
        public static VideosEntry getNext() {
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
       
        public static void save(String playlistname) {
            List<VideosEntry> newWideos = new List<VideosEntry>();
            for (int i = 0; i < playlistState.Count; i++)
            {
                newWideos.Add(playlistState[0]);
            }
            File.WriteAllText(directory, JsonConvert.SerializeObject(newWideos));
        }

        public static void load(String playlistname) {
            using (StreamReader readFromFile = new StreamReader(directory))
            {
                string json = readFromFile.ReadToEnd();
                List<VideosEntry> playlistLoad = JsonConvert.DeserializeObject<List<VideosEntry>>(json);
            }
        }
    }
}