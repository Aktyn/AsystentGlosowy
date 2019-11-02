using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using Asystent.common;

namespace Asystent.procedures {
	public class Example : ProcedureBase {
		public static Regex regex = new Regex(@"przyk[lł]adow[ae] (komenda|polecenie) ?(.*)",
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
				if( regex.IsMatch(res.result) ) {
					var match = regex.Match(res.result);
					String example_data = "";
					if (match.Success && match.Groups.Count > 0) {
						example_data = match.Groups[match.Groups.Count-1].Value;
					}
					Console.WriteLine("Executing example command with given data: \"" + example_data + '"');
					Finished = true;
					return;
				}
			}
		}
	}
}