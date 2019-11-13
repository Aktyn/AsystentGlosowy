using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Asystent.common;
using System.IO;

namespace Asystent.procedures 
{
    public struct SongRequestSchemaN
    {
        public string res { get; set; }
		public string video_id { get; set; }
		public string title { get; set; }
    }
    
    public class YoutubeNastepny : ProcedureBase
    {
        public static Regex regex = new Regex( @"(nastepny) ?(film|utwor|piosenkę)?",
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
					if (match.Success && match.Groups.Count > 0 && procedures.ProcedureBase.playlistMemory.Count > 0) 
                    {
                        SendData( new SongRequestSchemaN {
							res = procedures.ProcedureBase.playlistMemory[0].res, 
							video_id = procedures.ProcedureBase.playlistMemory[0].video_id, 
							title = procedures.ProcedureBase.playlistMemory[0].title
						} );

                        procedures.ProcedureBase.playlistMemory.RemoveAt(0);
						return;
                    }
                    else
                    {
                        Console.WriteLine("Pusta kolejka.");
                    }
                }
            }
        }
    }

}