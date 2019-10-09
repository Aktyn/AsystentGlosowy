using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using Asystent.common;

namespace Asystent.procedures {
	enum ResultType {
		INTERIM = 1,
		FINAL = 2,
		ALTERNATIVE = 3
	}

	public abstract class ProcedureBase {
		private static Type[] procedures = { typeof(Example) };
		public static Regex regex = null;

		protected List<SpeechResult> Results;
		protected bool Finished = false;

		public ProcedureBase(List<SpeechResult> results) {
			Results = results;
		}

		abstract public void Update(List<SpeechResult> results);

		public bool IsFinished() {
			return Finished;
		}
		
		public static List<ProcedureBase> MatchProcedures(List<SpeechResult> results) {
			List<ProcedureBase> matching = new List<ProcedureBase>();

			foreach (Type procedureClass in procedures) {
				FieldInfo regexField = procedureClass.GetField("regex",
					BindingFlags.Public | BindingFlags.Static);

				if (regexField != null) {
					Regex procedureRegex = (Regex) regexField.GetValue(null);

					//if at least one result matches regex - corresponding procedure is added to matching list
					foreach (SpeechResult res in results) {
						if (!procedureRegex.Match(res.result).Success) continue;
						matching.Add( (ProcedureBase)Activator.CreateInstance(procedureClass, results) );
						break;
					}
				}
				else {
					Console.WriteLine("Procedure must have static field \"regex\" of type Regex ({0})",
						procedureClass.Name);
				}
			}

			return matching;
		}
	}
}