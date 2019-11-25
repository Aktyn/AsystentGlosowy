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
	
	public delegate void SendDataListener(object data);

	public abstract class ProcedureBase {
		private static Type[] procedures = { typeof(Example), typeof(YoutubePlay), typeof(YoutubeVolume), typeof(YoutubeMute),
		 typeof(VolumeUnmute), typeof(YoutubeIncVolume), typeof(YoutubeDecVolume), typeof(YoutubeStop), typeof(YoutubeResume),
          typeof(YoutubePlaylistSaveAs), typeof(YoutubeNastepny),typeof(YoutubePlaylistLoad),
         };
		//public static Regex regex = null;

		protected List<SpeechResult> Results;
		protected bool Finished = false;
		
		public event SendDataListener OnSendData;

		protected ProcedureBase() { }

		abstract public void Update(List<SpeechResult> results);

		protected void SendData(object data) {
			OnSendData?.Invoke(data);
		}

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
						matching.Add( (ProcedureBase)Activator.CreateInstance(procedureClass) );
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