using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Asystent.common;
using System.IO;

namespace Asystent.procedures 
{    
    public class YoutubePlaylistAdd : ProcedureBase
    {
        public static Regex regex = new Regex( @"(zapisz) ?(piosenk[eę]|utw[oó]r)?",
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
                    if (match.Success && match.Groups.Count > 0) 
                    {
                        Playlist.add(PlayNow.title,PlayNow.video_id);
                        Playlist.counter++;
                        Playlist.Empty = false;
                    }
                }
            }
        }
    }

}