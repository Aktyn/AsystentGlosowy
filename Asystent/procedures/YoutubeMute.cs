using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Asystent.common;

namespace Asystent.procedures {
	public struct VolumeMuteSchema {
		public string res { get; set; }
	}
	public class YoutubeMute : ProcedureBase {
		public static Regex regex = new Regex(
			@"(wycisz|wy[lł][aą]cz) ?(d[zź]wi[eę]k)?",
			RegexOptions.IgnoreCase | RegexOptions.Compiled);

			public YoutubeMute() { }

			public sealed override void Update(List<SpeechResult> results) {
			Results = results;

			foreach (var res in results) {//ignore interim results
				if ((ResultType)res.type == ResultType.INTERIM)
					return;
			}
			results.Sort((res1, res2) => res1.confidence > res2.confidence ? 0 : 1);
			
			foreach (var res in results) {
				if (regex.IsMatch(res.result)) {
					SendData( new VolumeMuteSchema {
							res = "mute"
						} );
					Finished = true;
					return;
				}
			}
		}
	}
}