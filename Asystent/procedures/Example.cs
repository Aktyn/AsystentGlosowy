using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Asystent.common;

namespace Asystent.procedures {
	public class Example : ProcedureBase {
		public static new Regex regex = new Regex(@"przyk[lł]adow[ae] (komenda|polecenie) ?(.+)",
			RegexOptions.IgnoreCase | RegexOptions.Compiled);

		public Example(List<SpeechResult> results) : base(results) {
			Update(results);
		}
		
		public sealed override void Update(List<SpeechResult> results) {
			Results = results;
			
			Console.WriteLine("Executing example command");

			foreach (var res in results) {//ignore interim results
				if ((ResultType)res.type == ResultType.INTERIM)
					return;
			}
			
			//sort in place by confidence (DESC)
			results.Sort((res1, res2) => res1.confidence > res2.confidence ? 0 : 1);
			
			foreach (var res in results) {
				var match = regex.Match(res.result);
				//Console.WriteLine("{0} {1} ({2})", res.result, res.confidence, match.Groups.Count);
				if (match.Success && match.Groups.Count > 2) {//second parenthesis captured
					Console.WriteLine("Example additional text: " + match.Groups[2].Value);
				}
			}

			Finished = true;
		}
	}
}