using System;

namespace Asystent {
	class Program {
		private static void Main(string[] args) {
			string appName, youtubeApiKey;

			if (args.Length < 2) {
				Console.WriteLine("Program should be executed with ApplicationName and YoutubeApiKey arguments");
				Console.WriteLine("For egzample: program.exe VoiceAssistant qRzagddeDk51337RakJDN2g59oADkhtQnycn");
				
				//requesting arguments from console input
				Console.Write("ApplicationName: ");
				appName = Console.ReadLine();
				Console.Write("YoutubeApiKey: ");
				youtubeApiKey = Console.ReadLine();
			}
			else {
				appName = args[0];
				youtubeApiKey = args[1];
			}

			var connection = new ClientConnection();
			connection.OnMessage += MessageHandler.OnMessage;

			try {
				YouTube.Instance().Init(appName, youtubeApiKey);
				Console.WriteLine("YouTube Api initialized");
			}
			catch (Exception) {
				Console.WriteLine("Cannot initialize YouTube API");
			}

			//keep console alive and allow sending custom commands to frontend app
			var input = Console.ReadLine();
			while (input != "exit") {
				connection.DistributeMessage(input);
				input = Console.ReadLine();
			}
		}
	}
}