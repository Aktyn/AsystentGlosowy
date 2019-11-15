using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Asystent.common;
using System.IO;

namespace Asystent.procedures 
{    
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
                        Playlist.playlistMemory.Add(new Playlist.PlaylistSchema 
                        {
							res = "request_song", 
							video_id = video.id, 
							title = video.title
                        });
                        Console.WriteLine(Playlist.playlistMemory.Count);
                        Playlist.Empty = false;
                    }
                }
            }
        }
    }

}