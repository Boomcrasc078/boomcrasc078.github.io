using RaceTimer.Classes;
using System.Buffers.Text;
using System.Collections.Generic;

public class Race
{
	public string Id { get; set; } = string.Empty;
	public string Name { get; set; } = string.Empty;
	public List<Startlist> Startlists { get; set; } = new List<Startlist>();
	public DateTime creationDateTime { get; set; } = new DateTime();
	public DateTime lastEditDateTime { get; set; } = new DateTime();
	public string currentAnimation { get; set; } = "";

	public static string TimeSpanToString(TimeSpan timeSpan)
	{
		//return timeSpan.TotalSeconds.ToString();
		if (timeSpan.TotalSeconds < 60)
			return timeSpan.Seconds + " seconds ago";

		if (timeSpan.TotalMinutes < 60)
			return timeSpan.Minutes + " minutes ago";

		if (timeSpan.TotalHours < 24)
			return timeSpan.Hours + " hours ago";

		if (timeSpan.TotalDays < 7)
			return timeSpan.Days + " days ago";

		if (timeSpan.TotalDays < 30)
			return Math.Floor(timeSpan.TotalDays / 7) + " weeks ago";

		if (timeSpan.TotalDays < 365)
			return Math.Floor(timeSpan.TotalDays / 30) + " months ago";

		return Math.Floor(timeSpan.TotalDays / 365) + " years ago";
	}

	public Race DuplicateRace(IEnumerable<string> existingIds)
	{
		Race duplicatedRace = new Race()
		{
			Name = $"Copy of {this.Name}",
			Startlists = this.Startlists,
			creationDateTime = DateTime.Now,
			lastEditDateTime = DateTime.Now,
			Id = IdGenerator.GenerateUniqueId(existingIds)
		};
		return duplicatedRace;
	}

	public DateTime? FirstStartDatetime()
	{
		DateTime? datetime = null;
		foreach(var startlist in Startlists)
		{
			var getDatetime = startlist.FirstStartDateTime();
			if(getDatetime < datetime || datetime == null)
			{
				datetime = getDatetime;
			}
		}
		return datetime;
	}

	public List<DateTime> AllStartDatetime()
	{
		List<DateTime> datetimes = new();

		foreach(var startlist in Startlists)
		{
			var getStartlistDatetimes = startlist.AllStartDateTime();
			foreach(var datetime in getStartlistDatetimes)
			{
				if (datetimes.Contains(datetime))
				{
					continue;
				}
				datetimes.Add(datetime);
			}
		}
		return datetimes;
	}

	public class TimingEvent
	{
		public Racer Racer { get; set; } = new Racer();
		public DateTime DateTime { get; set; }
		public TimingSpot TimingSpot { get; set; }

		public TimingEvent(Racer racer, DateTime dateTime, TimingSpot timingSpot)
		{
			this.Racer = racer;
			this.DateTime = dateTime;
			TimingSpot = timingSpot;
		}
	}

	public class TimingSpot
	{
		public string Id { get; set; }
		public string Name { get; set; }

		public TimingSpot(string name, IEnumerable<string> existingIds)
		{
			this.Name = name;
			Id = IdGenerator.GenerateUniqueId(existingIds);
		}
	}
}
