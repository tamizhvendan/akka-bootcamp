﻿namespace WinTail
{
	using Akka.Actor;

	#region Program
	class Program
	{
		public static ActorSystem MyActorSystem;

		static void Main(string[] args)
		{
			
			MyActorSystem = ActorSystem.Create("MyActorSystem");

			
			var consoleWriterActor = MyActorSystem.ActorOf(Props.Create(() => new ConsoleWriterActor()));
			var consoleReaderActor = MyActorSystem.ActorOf(Props.Create(() => new ConsoleReaderActor(consoleWriterActor)));

			
			consoleReaderActor.Tell(ConsoleReaderActor.StartCommand);

			MyActorSystem.AwaitTermination();
		}
	}
	#endregion
}
