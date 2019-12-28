using System.Text.RegularExpressions;
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

        private static readonly string playlistsDir = Path.GetFullPath(
            Path.Combine(Directory.GetCurrentDirectory(), "..", "playlists")
        );

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

        public static void updatePlaylistsList() {
            string[] playlists = Directory.GetFiles(playlistsDir);
            for(int i=0; i<playlists.Length; i++) {
                string[] exploded = Regex.Split(playlists[i], @"(\\|/)");//playlists[i].Split("\\");
                string[] fileExploded = exploded[exploded.Length-1].Split(".");
                playlists[i] = fileExploded[0];
            }

            ClientConnection.Instance().DistributeMessage(JsonConvert.SerializeObject(new PlaylistsListUpdate{
                res = "playlists_list_update",
                playlists = playlists
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

        private static void preparePlaylistsDir() {
            bool exists = System.IO.Directory.Exists(playlistsDir);
            if(!exists)
                System.IO.Directory.CreateDirectory(playlistsDir);
        }
       
        public static void save(String playlistname) {
            preparePlaylistsDir();

            string filePath = playlistsDir + "/" + playlistname + ".json";
            Console.WriteLine(filePath);

            List<VideosEntry> newWideos = new List<VideosEntry>();
            newWideos.Add(current);
            for (int i = 0; i < playlistState.Count; i++)
            {
                newWideos.Add(playlistState[0]);
            }
            File.WriteAllText(filePath, JsonConvert.SerializeObject(newWideos));

            updatePlaylistsList();

            //TODO: notification
        }

        public static void load(String playlistname) {
            preparePlaylistsDir();

            string filePath = playlistsDir + "/" + playlistname + ".json";

            using (StreamReader readFromFile = new StreamReader(filePath))
            {
                string json = readFromFile.ReadToEnd();
                playlistState = JsonConvert.DeserializeObject<List<VideosEntry>>(json);
                
                ClientConnection.Instance().DistributeMessage(JsonConvert.SerializeObject(new SongRequestSchema {
					res = "request_song", 
					videos = Playlist.getNext()
				}));
            }
        }
    }
}