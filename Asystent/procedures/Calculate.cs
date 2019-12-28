using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Asystent.common;
using Asystent.procedures.calculateUtils;

namespace Asystent.procedures {
	public class Calculate : ProcedureBase {
		public static Regex regex = new Regex(@"^oblicz ([a-z]+ )?\d+[.,]?\d*",
			RegexOptions.IgnoreCase | RegexOptions.Compiled);

		public Calculate() { }

		private static Dictionary<string, string> operations = new Dictionary<string, string>
		{
			{"add", "+"},
			{"subtract", "-"},
			{"multiply", "*"},
			{"divide", "/"},
			{"power", "^"},
			{"factorial", "!"},
			{"sinus", "sin"},
			{"cosinus", "cos"},
			{"tangent", "tan"},
		};

		private static Dictionary<string, string[]> keywordReplacements = new Dictionary<string, string[]>
		{
			{operations["add"], new string[]{@"plus", @"doda[cć]"}},
			{operations["subtract"], new string[]{@"minus", @"odj[aą][cć]"}},
			{operations["multiply"], new string[]{@"razy", @"x"}},
			{operations["divide"], new string[]{@"podzieli[cć] (na|przez)?"}},
			{operations["factorial"], new string[]{@"silnia"}},

			{operations["power"]+"2", new string[]{@"(do)? kwadratu?"}},
			{operations["power"]+"3", new string[]{@"(do)? sze[sś]cianu?"}},

			{"0", new string[]{@"zero", @"zerowej"}},
			{"1", new string[]{@"jeden", @"pierwszej"}},
			{"2", new string[]{@"dwa", @"drugiej"}},
			{"3", new string[]{@"trzy", @"trzeciej"}},
			{"4", new string[]{@"cztery", @"czwartej"}},
			{"5", new string[]{@"pi[eę][cć]", @"pi[aą]tej"}},
			{"6", new string[]{@"sze[ss][cć]", @"sz[oó]stej"}},
			{"7", new string[]{@"siedem", @"si[oó]dmej"}},
			{"8", new string[]{@"osiem", @"[oó]smej"}},
			{"9", new string[]{@"dzwie[eę][cć]", @"dziew[aą]tej"}},
			{"10", new string[]{@"dziesi[eę][cć]", @"dziesi[aą]tej"}},
		};

		private delegate string TryToMatchKeyword(string input);
		private static Dictionary<string, TryToMatchKeyword> keywordReplacementsFunc = new Dictionary<string, TryToMatchKeyword> 
		{
			{operations["power"], (string input) => {
				var matchRegex = new Regex(@"do ([^ ]+) pot[eę]gi", RegexOptions.IgnoreCase | RegexOptions.Compiled);
				var match = matchRegex.Match(input);
				
				if(match.Success) {
					var i = match.Index;
					return input.Substring(0, i) + operations["power"] + match.Groups[1].Value + input.Substring(i + match.Length);
				}
				return input;
			}},
			{operations["sinus"], (string input) => {
				var matchRegex = new Regex(@"sinus (\d+[.,]?\d*) stopni", RegexOptions.IgnoreCase | RegexOptions.Compiled);
				var match = matchRegex.Match(input);
				
				if(match.Success) {
					var i = match.Index;
					return input.Substring(0, i) + "sin(" + match.Groups[1].Value + ")" + input.Substring(i + match.Length);
				}
				return input;
			}},
			{operations["cosinus"], (string input) => {
				var matchRegex = new Regex(@"cosinus (\d+[.,]?\d*) stopni", RegexOptions.IgnoreCase | RegexOptions.Compiled);
				var match = matchRegex.Match(input);
				
				if(match.Success) {
					var i = match.Index;
					return input.Substring(0, i) + "cos(" + match.Groups[1].Value + ")" + input.Substring(i + match.Length);
				}
				return input;
			}},
			{operations["tangent"], (string input) => {
				var matchRegex = new Regex(@"tangens (\d+[.,]?\d*) stopni", RegexOptions.IgnoreCase | RegexOptions.Compiled);
				var match = matchRegex.Match(input);
				
				if(match.Success) {
					var i = match.Index;
					return input.Substring(0, i) + "tan(" + match.Groups[1].Value + ")" + input.Substring(i + match.Length);
				}
				return input;
			}}
		};

		public sealed override void Update(List<SpeechResult> results) {
			Results = results;

			foreach (var res in results) {//ignore interim results
				if ((ResultType)res.type == ResultType.INTERIM)
					return;
			}
			
			//sort in place by confidence (DESC)
			results.Sort((res1, res2) => res1.confidence > res2.confidence ? 0 : 1);
			
			foreach (var res in results) {
				if( isMathCommand(res.result) ) {
					handleMathCommand(res.result);
					return;
				}
			}
		}

		private void handleMathCommand(String formatted_sentence) {
			foreach(var entry in keywordReplacements) {
				foreach(var regexpStr in entry.Value) {
					formatted_sentence = Regex.Replace(formatted_sentence, regexpStr, entry.Key);
				}
			}

			foreach(var entry in keywordReplacementsFunc) {
				try {
					formatted_sentence = entry.Value(formatted_sentence);
				}
				catch(Exception) {}
			}

			//clean
			List<string> operationSymbols = new List<string>();
			foreach(var op in operations.Values) {
				operationSymbols.Add( Regex.Escape(op) );
			}
			
			string symbolsRegexBase = @"\d+[.,]?\d*|\(|\)";
			foreach(var os in operationSymbols)
				symbolsRegexBase += "|" + os;

			var symbolsRegex = new Regex(@"(" + symbolsRegexBase + ")", RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);

			var formattedSentenceMatch = symbolsRegex.Matches(formatted_sentence);
			
			formatted_sentence = "";
			for(int matchI = 0; matchI < formattedSentenceMatch.Count; matchI++) {
				if( !formattedSentenceMatch[matchI].Success )
					continue;
				formatted_sentence += formattedSentenceMatch[matchI].Value;
			}
			
			SendData( new CalculationInfixResult {
				res = "calculate_infix", 
				infix = formatted_sentence,
			});
			//var result = InfixCalculator.Calculate(formatted_sentence);
			//var equation = formatted_sentence + " = " + result;
			
			//Console.WriteLine("equation: " + equation);
			
			/*this.notification = {
				content: equation
			};
			this.answer = {
				message: equation,
				loud: true,
				loud_message: equation_sentence.result.replace(/^oblicz/, '') + ' = ' + result
			};
			this.finished = true;*/
			Finished = true;
		}

		private static bool isMathCommand(String text) {
			var noSpaceResult = Regex.Replace(text, @"\s+", "");
			var mathRegex = new Regex(@"\d+[.,]?\d*",
				RegexOptions.IgnoreCase | RegexOptions.Compiled);
			return mathRegex.IsMatch(noSpaceResult);
		}
	}
}