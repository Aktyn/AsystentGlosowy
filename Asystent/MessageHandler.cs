using System;
using System.Collections.Generic;
using System.Linq;
using Asystent.common;
using Asystent.procedures;
using Fleck;
using Newtonsoft.Json;

namespace Asystent {
	internal enum MessageType {
		SpeechResult = 1
	}

	public class MessageHandler {
		private static MessageHandler _handler;

		private List<SpeechResult> _results;
		private readonly ulong _index;
		private ProcedureBase _procedure;

		private MessageHandler(List<SpeechResult> results, ulong index) {
			_results = results;
			_index = index;
		}

		private void Update(List<SpeechResult> updatedResults, ulong index) {
			if( _index != index )
				_procedure = null;//DISCARD PROCEDURE
			_results = updatedResults;
		}

		private bool Execute(IWebSocketConnection clientConn) {
			if( _procedure != null ) {
				_procedure.Update(_results);
			}
			else {
				var matchingProcedures = ProcedureBase.MatchProcedures(_results);
			
				if (matchingProcedures.Count < 1)//no procedure matches result
					return false;
			
				//console.log(matching_procedures);
			
				if (matchingProcedures.Count > 1) {
					Console.WriteLine("More than one procedure has been matched in single result");
					//TODO: list those procedure names
					return false;
				}

				_procedure = matchingProcedures.First();
				_procedure.OnSendData += data => {
					clientConn.Send(JsonConvert.SerializeObject(data));
				};
				
				_procedure.Update(_results);
			}
		
			return _procedure.IsFinished();
		}

		private ProcedureBase GetProcedure() {
			return _procedure;
		}

		private static SpeechResponse HandleSpeechResult(List<SpeechResult> results, ulong index, 
			IWebSocketConnection clientConn) 
		{
			if( _handler != null )
				_handler.Update(results, index);
			else
				_handler = new MessageHandler(results, index);

			if (!_handler.Execute(clientConn)) return new SpeechResponse {res = "ignored"};
			
			ProcedureBase procedure = _handler.GetProcedure();
			if (procedure == null)
				_handler = null;
			return new SpeechResponse{res = "executed", index = index};
		}
		
		public static void OnMessage(string message, IWebSocketConnection clientConn) {
			Console.WriteLine("Message: " + message);
			//example message:
			//"{"type":0,"results":[{"result":"zagraj costam","confidence":0.618,"type":2}],"result_index":0}"

			try {
				//var data = JObject.Parse(message);
				MessageSchema data = JsonConvert.DeserializeObject<MessageSchema>(message);
				object response = null;

				switch ((MessageType) data.type) {
					case MessageType.SpeechResult:
						response = HandleSpeechResult(data.results, data.result_index, clientConn);
						break;
					default:
						Console.WriteLine("Incorrect message format");
						break;
				}

				if (response != null)
					clientConn.Send(JsonConvert.SerializeObject(response));
			}
			catch (JsonReaderException) {
				Console.WriteLine("Cannot parse message as JSON");
			}
		}
	}
}