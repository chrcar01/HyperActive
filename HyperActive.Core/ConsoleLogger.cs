using System;

namespace HyperActive.Core
{
	public class ConsoleLogger : ILogger
	{

		public void Write(string text, params object[] args)
		{
			Console.Write(text, args);
		}

		public void WriteLine(string text, params object[] args)
		{
			Console.WriteLine(text, args);
		}
	}
}
