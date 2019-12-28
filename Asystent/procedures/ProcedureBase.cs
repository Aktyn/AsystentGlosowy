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

	public enum ConfirmationResult {
		UNKNOWN,
		CONFIRMED,
		REJECTED
	}
	
	public delegate void SendDataListener(object data);

	public abstract class ProcedureBase {
		private static Type[] procedures = { 
			typeof(ExampleConfirmation), typeof(YoutubePlay), typeof(YoutubeVolume),
			typeof(YoutubeMute), typeof(VolumeUnmute), typeof(YoutubeIncVolume),
			typeof(YoutubeDecVolume), typeof(YoutubeStop), typeof(YoutubeResume),
			typeof(YoutubePlaylistSaveAs), typeof(YoutubeNext), typeof(YoutubePlaylistLoad),
			typeof(Calculate)
         };
		private static Regex confirmRegex = new Regex(@"(tak|potwierd[zź]|potwierdzam|zatwierd[zź])",
			RegexOptions.IgnoreCase | RegexOptions.Compiled);
		private static Regex rejectRegex = new Regex(@"(nie|odrzuć|anuluj)",
			RegexOptions.IgnoreCase | RegexOptions.Compiled);

		private static ProcedureBase procedureRequestingConfirmation = null;

		protected List<SpeechResult> Results;
		protected bool Finished = false;
		
		public event SendDataListener OnSendData;

		protected ProcedureBase() { }

		abstract public void Update(List<SpeechResult> results);

		protected void SendData(object data) {
			OnSendData?.Invoke(data);
		}

		protected void RequestConfirmation(String dialogContent = "") {
			SendData(new ConfirmationRequestSchema {
								res = "request_confirmation", 
								dialog_content = dialogContent
			});
			procedureRequestingConfirmation = this;

			Utils.setTimeout(() => {
				if(procedureRequestingConfirmation != this)
					return;
				Console.WriteLine("Confirmation timed out");
				Reject();
				SendData(new SimpleResponse {
					res = "confirmation_timed_out"
				});
			}, 1000 * 15);//15 seconds
		}

		public virtual void onConfirm() {}
		public virtual void onReject() {}

		public bool IsFinished() {
			return Finished;
		}

		public static bool IsConfirmationRequested() {
			return procedureRequestingConfirmation != null;
		}

		public static ConfirmationResult ParseConfirmation(List<SpeechResult> results) {
			foreach (var res in results) {//ignore interim results
				if ((ResultType)res.type == ResultType.INTERIM)
					return ConfirmationResult.UNKNOWN;
			}
			
			//sort in place by confidence (DESC)
			results.Sort((res1, res2) => res1.confidence > res2.confidence ? 0 : 1);

			foreach (var res in results) {
				if( confirmRegex.IsMatch(res.result) ) {
					return ConfirmationResult.CONFIRMED;
				}
				else if( rejectRegex.IsMatch(res.result) ) {
					return ConfirmationResult.REJECTED;
				}
			}
			return ConfirmationResult.UNKNOWN;
		}

		public static void Confirm() {
			if(procedureRequestingConfirmation != null) {
				procedureRequestingConfirmation.onConfirm();
				procedureRequestingConfirmation.Finished = true;
				procedureRequestingConfirmation = null;
			}
		}

		public static void Reject() {
			if(procedureRequestingConfirmation != null) {
				procedureRequestingConfirmation.onReject();
				procedureRequestingConfirmation.Finished = true;
				procedureRequestingConfirmation = null;
			}
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