using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Console_Task_Manager
{
	class Program
	{
		static void Main(string[] args)
		{
			string command = "";
			List<TaskModel> tasksList = new List<TaskModel>();

			if (Directory.Exists("Lists") == false)
				Directory.CreateDirectory("Lists");

			ConsoleEx.WriteLine("--- Konsolowy Menadżer Zadań ---\n", ConsoleColor.Yellow);

			do
			{
				ConsoleEx.WriteLine("Wpisz komendę, lub help:", ConsoleColor.Green);
				command = Console.ReadLine().Trim().ToLower();
				Console.Clear();

				if (command == "exit" || command == "e")
					break;

				switch (command)
				{
					case "addtask":
					case "a":
						AddTask(tasksList);
						break;
					case "removetask":
					case "r":
						RemoveTask(tasksList);
						break;
					case "changetask":
					case "c":
						ChangeTask(tasksList);
						break;
					case "showtasks":
					case "st":
						ShowTasks(tasksList);
						break;
					case "showlists":
					case "sl":
						ShowLists();
						break;
					case "savetofile":
					case "s":
						SaveTasks(tasksList);
						break;
					case "loadfromfile":
					case "l":
						LoadTasks(tasksList);
						break;
					case "help":
					case "h":
						Help();
						break;
					case "showword":
					case "sw":
						SearchByWord(tasksList);
						break;
					default:
						ConsoleEx.WriteLine($"\"{command}\" -> To nieprawidłowa komenda!!!\n", ConsoleColor.Red);
						break;
				}
				ConsoleEx.WriteLine(" Nowe Polecenie ".PadLeft(24, '-').PadRight(32, '-') + "\n", ConsoleColor.Yellow);
			}
			while (true);
		}

		// --------------------------------------------------------------
		// Metody pod "DRY"
		// --------------------------------------------------------------

		/// <summary>
		/// 	Początkowe napisy pod wyświetlanie zadań
		/// 	Wykorzystywane przez: SearchByWord() i ShowTasks()
		/// </summary>

		private static void ShowSearchHeader()
		{
			ConsoleEx.WriteLine(" Aktualne Zadania ".PadLeft(25, '-').PadRight(32, '-'), ConsoleColor.Cyan);
			Console.WriteLine("(Jeżeli w nawiasach nie ma parametru to nie został on podany)");
		}

		/// <summary>
		/// 	Wyświetlanie zadań według kategorii
		/// 	Wykorzystywane przez: SearchByWord() i ShowTasks()
		/// </summary>
		/// <param name="tasksList"></param>
		/// <param name="wordForSearch"></param>

		private static void ShowTasksList(List<TaskModel> tasksList, string wordForSearch)
		{
			ConsoleEx.WriteLine("\nLista spóźnionych zadań:", ConsoleColor.Red);
			var lateTasksList = tasksList.Where(x => x.Description.Contains(wordForSearch))
						.Where(x => x.EndDate < DateTime.Now)
						.OrderBy(x => x.StartDate);
			foreach (TaskModel task in lateTasksList)
			{
				Console.Write($"Zadanie: {task.ToString()}\n");
			}

			ConsoleEx.WriteLine("\nLista ważnych zadań:", ConsoleColor.Red);
			var importantTasksList = tasksList.Where(x => x.IsTaskImportant == true)
						.Where(x => x.Description.Contains(wordForSearch))
						.Where(x => x.EndDate >= DateTime.Now || x.EndDate == null)
						.OrderBy(x => x.StartDate);
			foreach (TaskModel task in importantTasksList)
			{
				Console.Write($"Zadanie: {task.ToString()}\n");
			}

			ConsoleEx.WriteLine("\nPozostałe zadania:", ConsoleColor.Green);
			var notImportantTasksList = tasksList.Where(x => x.IsTaskImportant == false || x.IsTaskImportant == null)
						.Where(x => x.Description.Contains(wordForSearch))
						.Where(x => x.EndDate >= DateTime.Now || x.EndDate == null)
						.OrderBy(x => x.StartDate);
			foreach (TaskModel task in notImportantTasksList)
			{
				Console.Write($"Zadanie: {task.ToString()}\n");
			}

			ConsoleEx.WriteLine("Sukces!\n", ConsoleColor.Green);
		}

		/// <summary>
		/// 	Wypisywanie zadań z indeksami
		/// 	Wykorzystywane przez: ChangeTask() i AddTask()
		/// </summary>
		/// <param name="tasksList"></param>

		private static void RemoveAndChangePart(List<TaskModel> tasksList)
		{
			Console.WriteLine("\nOto lista zadań z indeksami:\n");
			for (int i = 0; i < tasksList.Count; i++)
			{
				Console.Write($"index {i}: {tasksList[i].ToString()}\n");
			}
		}

		/// <summary>
		/// 	Pobieranie zadania od użytkownika
		/// 	Wykorzystywane przez: ChangeTask() i AddTask()
		/// </summary>

		private static TaskModel GetTaskFromUserPart()
		{
			Console.Write("\nPodaj opis zadania: ");
			string description = Console.ReadLine().Trim();

			Console.Write("Podaj datę rozpoczęcia w formacie (RRRR-MM-DD GG:MM:SS): ");
			DateTime startDate;
			try
			{
				string startDateString = Console.ReadLine().Trim();

				if (startDateString.Length == 19)
				{
					startDate = DateTime.Parse(startDateString);
				}
				else
				{
					throw new FormatException();
				}
			}
			catch (FormatException)
			{
				ConsoleEx.WriteLine("Zadanie niewykonane!!! Datę należy podać według podanego schematu!!!\n", ConsoleColor.Red);
				return null;
			}

			Console.Write("Podaj datę zakończenia w formacie (RRRR-MM-DD GG:MM:SS), lub wciśnij enter, gdy nie znasz: ");
			DateTime? endDate;
			string endDateString = Console.ReadLine().Trim();
			if (string.IsNullOrWhiteSpace(endDateString))
			{
				endDate = null;
			}
			else
			{
				try
				{
					if (endDateString.Length == 19)
					{
						endDate = DateTime.Parse(endDateString);
					}
					else
					{
						throw new FormatException();
					}
				}
				catch (FormatException)
				{
					ConsoleEx.WriteLine("Zadanie niewykonane!!! Datę należy podać według podanego schematu!!!\n", ConsoleColor.Red);
					return null;
				}

			}

			Console.Write("Czy to zadanie na cały dzień? (Y/N) - lub wciśnij enter, gdy nie wiesz: ");
			bool? flag_fullDayQuest;
			string fullDayQuestString = Console.ReadLine().Trim();
			if (fullDayQuestString.ToUpper() == "Y")
			{
				flag_fullDayQuest = true;
			}
			else if (fullDayQuestString.ToUpper() == "N")
			{
				flag_fullDayQuest = false;
			}
			else
			{
				flag_fullDayQuest = null;
			}

			Console.Write("Czy to zadanie jest ważne? (Y/N) - lub wciśnij enter, gdy nie wiesz: ");
			bool? flag_taskIsImportant;
			string taskIsImportantString = Console.ReadLine().Trim();
			if (taskIsImportantString.ToUpper() == "Y")
			{
				flag_taskIsImportant = true;
			}
			else if (taskIsImportantString.ToUpper() == "N")
			{
				flag_taskIsImportant = false;
			}
			else
			{
				flag_taskIsImportant = null;
			}

			return new TaskModel(description, startDate, endDate, flag_fullDayQuest, flag_taskIsImportant);
		}

		// --------------------------------------------------------------
		// Metody prywatne - do działania programu
		// --------------------------------------------------------------

		/// <summary>
		/// 	Funkcja zmieniająca wybrane zadanie
		/// </summary>
		/// <param name="tasksList"></param>

		private static void ChangeTask(List<TaskModel> tasksList)
		{
			ConsoleEx.WriteLine(" Zmienianie Zadania ".PadLeft(26, '-').PadRight(32, '-'), ConsoleColor.Cyan);

			RemoveAndChangePart(tasksList);

			ConsoleEx.Write(("\n" + "Podaj index zadania do zmienienia, lub enter by anulować: "), ConsoleColor.Red);
			string ChosedIndex = Console.ReadLine().Trim();

			if (string.IsNullOrWhiteSpace(ChosedIndex))
			{
				ConsoleEx.WriteLine("Niczego nie zmieniono!\n", ConsoleColor.Green);
				return;
			}

			TaskModel task = GetTaskFromUserPart();

			if (task != null)
			{
				try
				{
					tasksList[(int.Parse(ChosedIndex))] = task;
					ConsoleEx.WriteLine("Sukces!\n", ConsoleColor.Green);
				}
				catch (FormatException)
				{
					ConsoleEx.WriteLine("Zadanie niewykonane!!! Indeks podajemy jako liczbę!!!\n", ConsoleColor.Red);
				}
			}
		}

		/// <summary>
		/// 	Funkcja szukająca zadań po podanym słowie
		/// </summary>
		/// <param name="tasksList"></param>

		private static void SearchByWord(List<TaskModel> tasksList)
		{
			ShowSearchHeader();

			Console.Write("\nPodaj słowo, według którego przeszukiwać opis: ");
			string wordForSearch = Console.ReadLine().Trim();

			ShowTasksList(tasksList, wordForSearch);
		}

		/// <summary>
		/// 	Funkcja pokazująca dostępne listy z zadaniami
		/// </summary>

		private static void ShowLists()
		{
			ConsoleEx.WriteLine(" Dostępne Listy ".PadLeft(24, '-').PadRight(32, '-'), ConsoleColor.Cyan);
			Console.WriteLine("\nOto dostępne listy:\n");

			string[] lists = Directory.GetFiles("Lists");
			foreach (string fileInfo in lists)
			{
				Console.WriteLine("--> " + fileInfo.Split('\\')[1]);
			}

			ConsoleEx.WriteLine("Sukces!\n", ConsoleColor.Red);
		}

		/// <summary>
		/// 	Funkcja pokazująca treść komendy help
		/// </summary>

		private static void Help()
		{
			ConsoleEx.WriteLine(" Help ".PadLeft(19, '-').PadRight(32, '-'), ConsoleColor.Cyan);
			Console.WriteLine("\nOto lista komend:\n");
			Console.WriteLine("help         - skrót 'h'  -> Wyświetla listę dostępnych komend.");
			Console.WriteLine("exit         - skrót 'e'  -> Wychodzi z programu.\n");
			Console.WriteLine("addtask      - skrót 'a'  -> Dodaje nowe zadanie.");
			Console.WriteLine("removetask   - skrót 'r'  -> Usuwa wybrane zadanie.");
			Console.WriteLine("changetask   - skrót 'c'  -> Zmienia wybrane zadanie.\n");
			Console.WriteLine("showtasks    - skrót 'st' -> Pokazuje wszystkie zadania z listy.");
			Console.WriteLine("showword     - skrót 'sw' -> Pokazuje wszystkie zadania z listy, które zawierają podane słowo w opisie.");
			Console.WriteLine("showlists    - skrót 'sl' -> Pokazuje wszystkie zapisane listy.\n");
			Console.WriteLine("savetofile   - skrót 's'  -> Zapisuje aktualną listę do pliku csv.");
			Console.WriteLine("loadfromfile - skrót 'l'  -> Wczytuje listę z pliku csv.");

			ConsoleEx.WriteLine("Sukces!\n", ConsoleColor.Green);
		}

		/// <summary>
		/// 	Funkcja ładująca podaną listę z pliku
		/// </summary>
		/// <param name="tasksList"></param>

		private static void LoadTasks(List<TaskModel> tasksList)
		{
			ConsoleEx.WriteLine(" Wczytywanie do Pliku ".PadLeft(27, '-').PadRight(32, '-'), ConsoleColor.Cyan);
			Console.Write("\nPodaj nazwę pliku (bez rozszerzenia): ");
			string myPath = Console.ReadLine().Trim().Replace(@"\", "_").Replace("/", "_").Replace(":", "_").Replace("*", "_").Replace("?", "_")
					.Replace("\"", "_").Replace("<", "_").Replace(@">", "_").Replace("|", "_"); ;

			if (File.Exists(@"Lists/" + myPath + ".csv") == false)
			{
				ConsoleEx.WriteLine("Plik nie istnieje!!!\n", ConsoleColor.Red);
				return;
			}

			string[] linesFromFile = File.ReadAllLines(@"Lists/" + myPath + ".csv");

			List<string[]> tasksInStringList = new List<string[]>();
			foreach (string fullCSV in linesFromFile)
			{
				tasksInStringList.Add(fullCSV.Split(';'));
			}
			try
			{
				foreach (string[] taskInParts in tasksInStringList)
				{
					DateTime? endDate;
					if (string.IsNullOrWhiteSpace(taskInParts[2]))
					{
						endDate = null;
					}
					else
					{
						endDate = DateTime.Parse(taskInParts[2]);
					}

					bool? forAllDay;
					if (string.IsNullOrWhiteSpace(taskInParts[3]))
					{
						forAllDay = null;
					}
					else
					{
						forAllDay = bool.Parse(taskInParts[3]);
					}

					bool? importantTask;
					if (string.IsNullOrWhiteSpace(taskInParts[4]))
					{
						importantTask = null;
					}
					else
					{
						importantTask = bool.Parse(taskInParts[4]);
					}

					tasksList.Add(new TaskModel(taskInParts[0], DateTime.Parse(taskInParts[1]), endDate, forAllDay, importantTask));
				}

				ConsoleEx.WriteLine("Sukces!", ConsoleColor.Green);
				ConsoleEx.WriteLine("Lista została wczytana!\n", ConsoleColor.Green);
			}
			catch (FormatException)
			{
				tasksList.Clear();
				ConsoleEx.WriteLine("Zadanie niewykonane!!! Ktoś popsuł plik!!!\n", ConsoleColor.Red);
			}
			catch (IndexOutOfRangeException)
			{
				tasksList.Clear();
				ConsoleEx.WriteLine("Zadanie niewykonane!!! Ktoś popsuł plik!!!\n", ConsoleColor.Red);
			}
		}

		/// <summary>
		/// 	Funkcja zapisująca aktualną listę do pliku
		/// </summary>
		/// <param name="tasksList"></param>

		private static void SaveTasks(List<TaskModel> tasksList)
		{
			ConsoleEx.WriteLine(" Zapis do Pliku ".PadLeft(24, '-').PadRight(32, '-'), ConsoleColor.Cyan);
			Console.Write("\nPodaj nazwę dla swojej listy (bez rozszerzenia): ");
			string myPath = Console.ReadLine().Trim().Replace(@"\", "_").Replace("/", "_").Replace(":", "_").Replace("*", "_").Replace("?", "_")
					.Replace("\"", "_").Replace("<", "_").Replace(@">", "_").Replace("|", "_");

			if (File.Exists(@"Lists/" + myPath + ".csv"))
			{
				ConsoleEx.WriteLine("Plik istnieje!!!\n", ConsoleColor.Red);
				return;
			}

			List<string> tasksInStringList = new List<string>();

			foreach (TaskModel task in tasksList)
			{
				tasksInStringList.Add(task.ToCsv());
			}

			File.WriteAllLines((@"Lists/" + myPath + ".csv"), tasksInStringList);
			ConsoleEx.WriteLine("Sukces!", ConsoleColor.Green);
			tasksList.Clear();
			ConsoleEx.WriteLine("Lista została wyczyszczona!\n", ConsoleColor.Green);
		}

		/// <summary>
		/// 	Funkcja pokazująca wszystkie zadania
		/// </summary>
		/// <param name="tasksList"></param>

		private static void ShowTasks(List<TaskModel> tasksList)
		{
			ShowSearchHeader();
			ShowTasksList(tasksList, "");
		}

		/// <summary>
		/// 	Funkcja usuwająca wybrane zadanie
		/// </summary>
		/// <param name="tasksList"></param>

		private static void RemoveTask(List<TaskModel> tasksList)
		{
			ConsoleEx.WriteLine(" Usuwanie Zadania ".PadLeft(25, '-').PadRight(32, '-'), ConsoleColor.Cyan);

			RemoveAndChangePart(tasksList);

			ConsoleEx.Write(("\n" + "Podaj index zadania do usunięcia, lub enter by anulować: "), ConsoleColor.Red);
			string ChosedIndex = Console.ReadLine().Trim();

			if (string.IsNullOrWhiteSpace(ChosedIndex))
			{
				ConsoleEx.WriteLine("Niczego nie usunięto!\n", ConsoleColor.Green);
				return;
			}

			try
			{
				tasksList.RemoveAt(int.Parse(ChosedIndex));
				ConsoleEx.WriteLine("Sukces!\n", ConsoleColor.Green);
			}
			catch (FormatException)
			{
				ConsoleEx.WriteLine("Zadanie niewykonane!!! Indeks podajemy jako liczbę!!!\n", ConsoleColor.Red);
			}
		}

		/// <summary>
		/// 	Funkcja dodająca nowe zadanie do listy
		/// </summary>
		/// <param name="tasksList"></param>

		private static void AddTask(List<TaskModel> tasksList)
		{
			ConsoleEx.WriteLine(" Dodawanie Nowego Zadania ".PadLeft(29, '-').PadRight(32, '-'), ConsoleColor.Cyan);

			TaskModel task = GetTaskFromUserPart();

			if (task != null)
			{
				tasksList.Add(task);
				ConsoleEx.WriteLine("Sukces!\n", ConsoleColor.Green);
			}
		}
	}
}
