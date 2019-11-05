using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Asystent.common;

namespace Asystent.procedures {
	public struct DecVolumeSchema {
		public string res { get; set; }
		public string volume { get; set; }
	}
	public class YoutubeDecVolume : ProcedureBase {
		public static Regex regex = new Regex(
			@"(zmniejsz|[sś]cisz|przycisz) ?(g[lł]o[sś]no[sś][cć]|d[zź]wi[eę]k)?",
			RegexOptions.IgnoreCase | RegexOptions.Compiled);

			public YoutubeDecVolume() { }

			public sealed override void Update(List<SpeechResult> results) {
			Results = results;

			foreach (var res in results) {//ignore interim results
				if ((ResultType)res.type == ResultType.INTERIM)
					return;
			}
			results.Sort((res1, res2) => res1.confidence > res2.confidence ? 0 : 1);
			
			foreach (var res in results) {
				if (regex.IsMatch(res.result)) {
					string [] split = res.result.Split(new Char [] {' '});
					string vol=null;
					
					for(int i=0; i<split.Length; i++)
					{
						if(Regex.IsMatch(split[i], @"^\d+$"))
						{
							vol=split[i];
							break;
						}
					}
					SendData( new DecVolumeSchema {
							res = "volume_down", 
							volume = vol
						} );
					Finished = true;
					return;
				}
			}
		}
	}
}