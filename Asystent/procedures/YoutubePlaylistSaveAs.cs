using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Asystent.common;
using System.Reflection;
using System.IO;

namespace Asystent.procedures
{
public class YoutubePlaylistSaveAs : ProcedureBase
    {
        public static Regex regex = new Regex(@"(Zapisz) ?(jako)? (.+)",
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
                    //zapisz jako
                     var match = regex.Match(res.result);
                     String example_data = @"zapisz\sjako\s(.+)";
                     if (match.Success && match.Groups.Count > 0)
                     {
                       example_data = match.Groups[match.Groups.Count - 1].Value;              
                     }
                    Playlist.save(example_data);
                    Console.WriteLine("Zapisano playlistę: \"" + example_data + '"');

                    Finished = true;
                    return ;
                }
            }
        }
    }

}
