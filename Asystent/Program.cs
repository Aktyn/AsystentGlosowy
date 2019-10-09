using System;

namespace Asystent {
	class Program {
		private static void Main() {
			var connection = new ClientConnection();
			connection.OnMessage += MessageHandler.OnMessage;

			//keeps console alive
			var input = Console.ReadLine();
			while (input != "exit") {
				connection.DistributeMessage(input);
				input = Console.ReadLine();
			}
		}
	}
}