using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Asystent.common;

namespace Asystent.procedures {
	public class Stats : ProcedureBase {
		public static Regex regex = new Regex(
			@"(zamknij|wy[lł][oaą]cz|poka[zż]|wy[sś]wietl|wypisz) ?(statystyki|statystyk[eę])? (.+)",
			RegexOptions.IgnoreCase | RegexOptions.Compiled);
		
		public static Regex regexStat = new Regex(
			@"(zamknij|wy[lł][oaą]cz)",
			RegexOptions.IgnoreCase | RegexOptions.Compiled);
		public Stats() { }
		
		public sealed override void Update(List<SpeechResult> results) {
			Results = results;

			foreach (var res in results) {//ignore interim results
				if ((ResultType)res.type == ResultType.INTERIM)
					return;
			}
			
			//sort in place by confidence (DESC)
			results.Sort((res1, res2) => res1.confidence > res2.confidence ? 0 : 1);
			
			foreach (var res in results) {
				if (regex.IsMatch(res.result)) {
					SendData(new SimpleResponse
                    {
						res = regexStat.IsMatch(res.result)?"closeStats":"showStats"
                    });
                    return;
				}
			}
		}
	}
}