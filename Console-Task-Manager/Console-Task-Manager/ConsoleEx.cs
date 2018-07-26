using System;

namespace Console_Task_Manager
{
	public static class ConsoleEx
	{
		public static void WriteLine(string text, ConsoleColor consoleColor)
		{
			Console.ForegroundColor = consoleColor;
			Console.WriteLine(text);
			Console.ResetColor();
		}

		public static void Write(string text, ConsoleColor consoleColor)
		{
			Console.ForegroundColor = consoleColor;
			Console.Write(text);
			Console.ResetColor();
		}
	}
}
