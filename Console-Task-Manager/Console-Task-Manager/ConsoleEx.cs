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

		public static void WriteLineInGreen(string text)
		{
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine(text);
			Console.ResetColor();
		}

		public static void WriteLineInRed(string text)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(text);
			Console.ResetColor();
		}

		public static void WriteLineInYellow(string text)
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine(text);
			Console.ResetColor();
		}

		public static void WriteLineInCyan(string text)
		{
			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.WriteLine(text);
			Console.ResetColor();
		}
	}
}
