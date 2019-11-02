using System;

namespace Asystent {
	class Program {
		//TODO: create list of available commands in welcome message
		private static string WelcomeMessage = "Example welcome message\n\tTODO - short help";

		private static void Main(string[] args) {
			string youtubeApiKey;

			if (args.Length < 1) {
				Console.WriteLine("Program should be executed with YoutubeApiKey argument");
				Console.WriteLine("For egzample: program.exe VoiceAssistant qRzagddeDk51337RakJDN2g59oADkhtQnycn");
				
				//requesting arguments from console input
				Console.Write("YoutubeApiKey: ");
				youtubeApiKey = Console.ReadLine();
			}
			else {
				youtubeApiKey = args[0];
			}

			//Establishing WebSocket connections
			var connection = new ClientConnection();
			connection.OnMessage += MessageHandler.OnMessage;
			connection.OnServerStart += () => {
				#if !DEBUG
				ChromeOpener.OpenInStandaloneWindow();
				#endif
			};

			connection.Connect();

			//Initializing YouTube API
			try {
				YouTube.Instance().Init("VoiceAssistant", youtubeApiKey);
				Console.WriteLine("YouTube Api initialized");
			}
			catch (Exception) {
				Console.WriteLine("Cannot initialize YouTube API");
			}

			Console.WriteLine("\n" + WelcomeMessage);
			do {//keep console alive and allow sending custom commands to frontend app
				var command = Console.ReadLine();

				switch(command) {
					default:
						connection.DistributeMessage(command);
						break;
					case "exit":
						return;
				}
			} while(true);
		}
	}
}