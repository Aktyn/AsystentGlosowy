using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Asystent.common;
using System.IO;

namespace Asystent.procedures 
{
    public struct PlaylistSchema
    {
        public string res { get; set; }
		public string video_id { get; set; }
		public string title { get; set; }
    }
    
    public class YoutubePlaylist : ProcedureBase
    {
        public static Regex regex = new Regex( @"(dodaj) (.+)",
			RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public override void Update(List<SpeechResult> results)
        {
            foreach (var res in results) 
            {
				if ((ResultType)res.type == ResultType.INTERIM)
					return;
			}

            results.Sort((res1, res2) => res1.confidence > res2.confidence ? 0 : 1);

            
            foreach (var res in results)
            {
                if (regex.IsMatch(res.result)) 
                {
                    var match = regex.Match(res.result);
                    var user_query = match.Groups[match.Groups.Count-1].Value;
                    VideoInfo video = YouTube.Instance().SearchVideo( user_query );
                    if (match.Success && match.Groups.Count > 0) 
                    {
                        playlistMemory.Add(new PlaylistSchema 
                        {
							res = "request_song", 
							video_id = video.id, 
							title = video.title
                        });
                        Console.WriteLine(playlistMemory.Count);
                        /*for(int i = 0; i < playlistMemory.Count; i++)
                            Console.Write(ProcedureBase.playlistMemory[i].video_id + " ");
                        */
                        /*try
                        {
                            using (StreamWriter save = new StreamWriter(("Playlist.txt"),true))
                            {
                                save.WriteLine(VideoInfo);
                            }
                        }*/
                    }
                }
            }
        }
    }

}