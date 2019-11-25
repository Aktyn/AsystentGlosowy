using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
        public static string directory = @"C:/Users/jarek/Desktop/Nowy folder (3)/Playlista.json";
        public static bool isEmpty()
        {
            if(playlistMemory.Count == 0)
                return true;
            return false;
        }
       
        public static void save(String playlistname)
        {
            List<VideoInfo> newWideo = new List<VideoInfo>();
            for (int i = 0; i < playlistMemory.Count; i++)
            {
                newWideo.Add(new VideoInfo()
                {
                    id = playlistMemory[i].id,
                    title = playlistMemory[i].title
                });
            }
            File.WriteAllText(directory, JsonConvert.SerializeObject(newWideo));
        }

        public static void load(String playlistname)
        {

            using (StreamReader readFromFile = new StreamReader(directory))
            {
                string json = readFromFile.ReadToEnd();
                List<VideoInfo> playlistLoad = JsonConvert.DeserializeObject<List<VideoInfo>>(json);
            }

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