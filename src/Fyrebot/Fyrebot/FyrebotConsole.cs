using System;

namespace Rogue.Fyrebot
{
	class FyrebotConsole : IConsole
	{
		public void WriteLine(ConsoleColor color, string message, params object[] parms)
		{
			try
			{
				Console.ForegroundColor = color;
				Console.WriteLine(message, parms);
			}
			finally
			{
				Console.ResetColor();
			}
		}

		public void WriteLine(string message, params object[] parms)
		{
			WriteLine(ConsoleColor.Gray, message, parms);
		}
	}
}