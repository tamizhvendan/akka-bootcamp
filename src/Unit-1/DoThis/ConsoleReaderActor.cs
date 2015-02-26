using System;
using Akka.Actor;

namespace WinTail
{
	using System.Runtime.Remoting.Messaging;

	/// <summary>
	/// Actor responsible for reading FROM the console. 
	/// Also responsible for calling <see cref="ActorSystem.Shutdown"/>.
	/// </summary>
	class ConsoleReaderActor : UntypedActor
	{
		public const string ExitCommand = "exit";
		public const string StartCommand = "start";
		private ActorRef _consoleWriterActor;

		public ConsoleReaderActor(ActorRef consoleWriterActor)
		{
			_consoleWriterActor = consoleWriterActor;
		}

		protected override void OnReceive(object message)
		{
			if (message.Equals(StartCommand))
			{
				DoPrintInstructions();
			}
			else if (message is InputError)
			{
				_consoleWriterActor.Tell(message as InputError);
			}

			GetAndValidateInput();
		}

		private void GetAndValidateInput()
		{
			var message = Console.ReadLine();
			if (string.IsNullOrEmpty(message))
			{
				// signal that the user needs to supply an input, as previously
				// received input was blank
				Self.Tell(new NullInputError("No input received."));
			}
			else if (String.Equals(message, ExitCommand, StringComparison.OrdinalIgnoreCase))
			{
				// shut down the entire actor system (allows the process to exit)
				Context.System.Shutdown();
			}
			else
			{
				var valid = IsValid(message);
				if (valid)
				{
					_consoleWriterActor.Tell(new InputSuccess("Thank you! Message was valid."));

					// continue reading messages from console
					Self.Tell(new ContinueProcessing());
				}
				else
				{
					Self.Tell(new ValidationError("Invalid: input had odd number of characters."));
				}
			}
		}

		private static bool IsValid(string message)
		{
			var valid = message.Length % 2 == 0;
			return valid;
		}

		private void DoPrintInstructions()
		{
			Console.WriteLine("Write whatever you want into the console!");
			Console.WriteLine("Some entries will pass validation, and some won't...\n\n");
			Console.WriteLine("Type 'exit' to quit this application at any time.\n");
		}
	}
}