using System;
using System.Collections.Generic;
using System.Linq;
using Asystent.common;
using Asystent.procedures;
using Fleck;
using Newtonsoft.Json;

namespace Asystent {
	internal enum MessageType {
		SpeechResult = 1,
		VideoFinished = 2
	}

	public class MessageHandler {
		private static MessageHandler _handler;

		private List<SpeechResult> _results;
		private ulong _index;
		private ProcedureBase _procedure;

		private MessageHandler(List<SpeechResult> results, ulong index) {
			_results = results;
			_index = index;
		}

		private void Update(List<SpeechResult> updatedResults, ulong index) {
			if( _index != index ) {
				_index = index;
				_procedure = null;//DISCARD PROCEDURE
			}
			_results = updatedResults;
		}

		private bool Execute(IWebSocketConnection clientConn) {
			if( _procedure == null ) {
				var matchingProcedures = ProcedureBase.MatchProcedures(_results);
			
				if (matchingProcedures.Count < 1)//no procedure matches result
					return false;
			
				//Console.WriteLine(matchingProcedures.Count);
			
				if (matchingProcedures.Count > 1) {
					Console.WriteLine("More than one procedure has been matched in single result");
					//TODO: list those procedure names
					return false;
				}

				_procedure = matchingProcedures.First();
				_procedure.OnSendData += data => {
					clientConn.Send(JsonConvert.SerializeObject(data));
				};
			}

			_procedure.Update(_results);
		
			return _procedure.IsFinished();
		}

		private ProcedureBase GetProcedure() {
			return _procedure;
		}

		public static void Clear() {
			_handler = null;
		}

		private static SpeechResponse HandleSpeechResult(SpeechMessageSchema data, 
			IWebSocketConnection clientConn) 
		{
			if( _handler != null )
				_handler.Update(data.results, data.result_index);
			else
				_handler = new MessageHandler(data.results, data.result_index);

			bool executed = _handler.Execute(clientConn);
			ProcedureBase procedure = _handler.GetProcedure();

			if (!executed) {
				var resp = new SpeechResponse {res = "ignored"};
				if(procedure != null)
					resp.procedure = procedure.GetType().Name;
				return resp;
			}
			
			if (procedure == null)
				_handler = null;
			Console.WriteLine("Command executed, sending response to client");
			return new SpeechResponse{res = "executed", index = data.result_index};
		}
		
		public static void OnMessage(string message, IWebSocketConnection clientConn) {
			Console.WriteLine("Message: " + message);
			//example message:
			//"{"type":1,"results":[{"result":"zagraj costam","confidence":0.618,"type":2}],"result_index":0}"

			try {
				object data = JsonConvert.DeserializeObject<MessageSchema>(message);
				object response = null;

				switch ((MessageType) ((MessageSchema)data).type) {
					case MessageType.SpeechResult:
						response = HandleSpeechResult(
							JsonConvert.DeserializeObject<SpeechMessageSchema>(message),
							clientConn
						);
						break;
					case MessageType.VideoFinished:
						Console.WriteLine("Finished video: " + JsonConvert.DeserializeObject<VideoFinishedMessageSchema>(message).video_id);
						
						if(!Playlist.isEmpty())
						{
							clientConn.Send(JsonConvert.SerializeObject(new SongRequestSchema {
								res = "request_song", 
								videos = Playlist.getNext()
							}));
							break;
						}
						else
						{
							Playlist.PlaylistEndSchema end = new Playlist.PlaylistEndSchema();
							end.res = "end_playlist";
							Playlist.current = null;
							clientConn.Send(JsonConvert.SerializeObject(end));

							Console.WriteLine("Pusta kolejka. <MessengeHandler>");
						}
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