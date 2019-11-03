using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Asystent.common;

namespace Asystent.procedures {
	public struct IncVolumeSchema {
		public string res { get; set; }
		public string volume { get; set; }
	}
	public class YoutubeIncVolume : ProcedureBase {
		public static Regex regex = new Regex(
			@"(podglośnij|podgłośnij|podglosnij|podgłosnij) ?(piosenk[eę]|utw[oó]r)? (.+)",
			RegexOptions.IgnoreCase | RegexOptions.Compiled);

			public YoutubeIncVolume() { }

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
					int i =0;
					while(i<5)
					{
						if(Regex.IsMatch(split[i], @"^\d+$"))
						{
							vol=split[i];
							break;
						}
						i++;
					}
					SendData( new IncVolumeSchema {
							res = "volume_up", 
							volume = vol
						} );
					Finished = true;
					return;
				}
			}
		}
	}
}