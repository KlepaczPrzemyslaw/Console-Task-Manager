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
		public bool? IsFullDayQuest { get; protected set; }
		// Not required
		public bool? IsTaskImportant { get; protected set; }

		public TaskModel(string description, DateTime startDate, DateTime? endDate, bool? isFullDayQuest, bool? isTaskImportant)
		{
			this.Description = description;
			this.StartDate = startDate;
			this.EndDate = endDate;
			this.IsFullDayQuest = isFullDayQuest;
			this.IsTaskImportant = isTaskImportant;

			if (endDate == null && isFullDayQuest == true)
			{
				this.EndDate = StartDate.AddDays(1);
			}
		}

		public override string ToString()
		{
			return	$"- Opis: ( {Description} )\n" +
					$"         - Start: ( {StartDate} )\n" +
					$"         - Koniec: ( {EndDate} )\n" +
					$"         - Czy to zadanie na cały dzień: ( {IsFullDayQuest} )\n" +
					$"         - Czy to ważne zadanie: ( {IsTaskImportant} )";
		}

		public string ToCsv()
		{
			return $"{Description};{StartDate};{EndDate};{IsFullDayQuest};{IsTaskImportant}";
		}
	}
}
