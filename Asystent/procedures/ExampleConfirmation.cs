using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Asystent.common;

namespace Asystent.procedures {
	public class ExampleConfirmation : ProcedureBase {
		public static Regex regex = new Regex(@"przyk[lł]adow[ae] (komenda|polecenie)",
			RegexOptions.IgnoreCase | RegexOptions.Compiled);

		public ExampleConfirmation() { }
		
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
					RequestConfirmation("Czy na pewno chcesz uruchomić przykładową komendę?");
					return;
				}
			}
		}

		override public void onConfirm() {
			Console.WriteLine("Procedure confirmed");
		}
		override public void onReject() {
			Console.WriteLine("Procedure rejected");
		}
	}
}