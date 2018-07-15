using System;

namespace Console_Task_Manager
{
	public class TaskModel
	{
		// Required
		public string Description { get; protected set; }
		// Required
		public DateTime StartDate { get; protected set; }
		// Not required
		public DateTime? EndDate { get; protected set; }
		// Not required
		public bool? Flag_FullDayQuest { get; protected set; }
		// Not required
		public bool? Flag_TaskIsImportant { get; protected set; }

		public TaskModel(string description, DateTime startDate, DateTime? endDate, bool? flag_fullDayQuest, bool? flag_taskIsImportant)
		{
			this.Description = description;
			this.StartDate = startDate;
			this.EndDate = endDate;
			this.Flag_FullDayQuest = flag_fullDayQuest;
			this.Flag_TaskIsImportant = flag_taskIsImportant;

			if (endDate == null && flag_fullDayQuest == true)
			{
				this.EndDate = StartDate.AddDays(1);
			}
		}

		public string GetTaskAsString()
		{
			return	$"- Opis: ( {Description} )\n" +
					$"         - Start: ( {StartDate} )\n" +
					$"         - Koniec: ( {EndDate} )\n" +
					$"         - Czy to zadanie na cały dzień: ( {Flag_FullDayQuest} )\n" +
					$"         - Czy to ważne zadanie: ( {Flag_TaskIsImportant} )";
		}

		public string GetTaskAsStringForCSV()
		{
			return $"{Description};{StartDate};{EndDate};{Flag_FullDayQuest};{Flag_TaskIsImportant}";
		}
	}
}
