using RaceTimer.Classes;
using System.Buffers.Text;
using System.Collections.Generic;

public class Race
{
    public string Id { get; set; }
    public string Name { get; set; }
    public List<Startlist> Startlists { get; set; } = new List<Startlist>();
    public DateTime creationDateTime { get; set; } = new DateTime();
    public DateTime lastEditDateTime { get; set; } = new DateTime();
    public string currentAnimation { get; set; } = "";

	public static string TimeSpanToString(TimeSpan timeSpan)
	{
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

	Race DuplicateRace()
	{
		Race duplicatedRace = new Race();

		duplicatedRace.Name = $"Copy of {this.Name}";

		return duplicatedRace;
	}
}
