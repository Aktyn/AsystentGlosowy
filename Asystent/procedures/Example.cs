using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Asystent.common;

namespace Asystent.procedures {
	public class Example : ProcedureBase {
		public static Regex regex = new Regex(@"przyk[lł]adow[ae] (komenda|polecenie)",
			RegexOptions.IgnoreCase | RegexOptions.Compiled);

		public Example() { }
		
		public sealed override void Update(List<SpeechResult> results) {
			Results = results;

			foreach (var res in results) {//ignore interim results
				if ((ResultType)res.type == ResultType.INTERIM)
					return;
			}
			
			//sort in place by confidence (DESC)
			results.Sort((res1, res2) => res1.confidence > res2.confidence ? 0 : 1);
			
			foreach (var res in results) {
				var match = regex.IsMatch(res.result);
				if (match) {
					Console.WriteLine("Executing example command");
					break;
				}
			}

			Finished = true;
		}
	}
}