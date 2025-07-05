using DocumentFormat.OpenXml.Drawing.Diagrams;
using RaceTimer.Classes.Timing;

namespace RaceTimer.Classes
{
	public class Racer
	{
		public string Name { get; set; } = string.Empty;
		public string Surname { get; set; } = string.Empty;
		public string Bib { get; set; } = string.Empty;
		public string Id { get; set; } = string.Empty;
		public DateTime? StartDateTime { get; set; } = null;
		public List<RacerEvent> Events { get; set; } = new List<RacerEvent>();
		public string currentAnimation { get; set; } = string.Empty;
		public List<CustomField> CustomFields { get; set; } = new List<CustomField>();

		public class CustomField
		{
			public string Name { get; set; }
			public string Data { get; set; }

			public CustomField(string name, string data)
			{
				Name = name;
				Data = data;
			}
		}

		// Helper to get all lap DateTimes
		private List<DateTime> GetLapDateTimes() => Events.Select(e => e.DateTime).ToList();

		string? GetTime()
		{
			var lapTimes = GetLapDateTimes();
			if (lapTimes.Count == 0)
				return null;
			if (StartDateTime == null)
				return null;
			var time = lapTimes.Last() - StartDateTime.Value;
			var timeString = time.ToString("hh\\:mm\\:ss\\.ff");
			return timeString;
		}

		string? GetPace(float distanceMeters)
		{
			var lapTimes = GetLapDateTimes();
			if (lapTimes.Count == 0)
				return null;
			if (StartDateTime == null)
				return null;
			var time = lapTimes.Last() - StartDateTime.Value;
			var laps = lapTimes.Count;
            var pace = time / (distanceMeters * laps / 1000);

			var stringPace = pace.ToString("mm\\:ss");
			return stringPace;
		}

		string? GetSpeed(float distanceMeters)
		{
			var lapTimes = GetLapDateTimes();
			if (lapTimes.Count == 0)
				return null;
			if (StartDateTime == null)
				return null;
			var time = lapTimes.Last() - StartDateTime.Value;
			var laps = lapTimes.Count;
            var speed = (distanceMeters * laps / 1000) / time.TotalHours;

			string stringSpeed = Double.Floor(speed).ToString();
			return stringSpeed;
		}

		List<string>? GetLaptimes()
		{
			var lapTimes = GetLapDateTimes();
			if (lapTimes.Count == 0)
				return null;
			if (StartDateTime == null)
				return null;

			List<TimeSpan> lapTimeSpans = new List<TimeSpan>();

			for (int i = 0; i < lapTimes.Count; i++)
			{
				if (i == 0)
				{
					lapTimeSpans.Add(lapTimes[i] - StartDateTime.Value);
					continue;
				}

				lapTimeSpans.Add(lapTimes[i] - lapTimes[i - 1]);
			}

			List<string> lapTimesString = lapTimeSpans.Select(x => x.ToString("hh\\:mm\\:ss\\.ff")).ToList();

			return lapTimesString;
		}

		public object? GetResultData(string key)
		{
			switch (key)
			{
				case "name": return Name;
				case "surname": return Surname;
				case "bib": return Bib;
				case "id": return Id;
				case "time": return GetTime();
				case "laps": return Events.Count;
				case "laptimes": return GetLaptimes();
				default: return null;
			}
		}

		public object? GetResultData(string key, float distanceMeters)
		{
			switch (key)
			{
				case "pace": return GetPace(distanceMeters);
				case "speed": return GetSpeed(distanceMeters);
				default: return null;
			}
		}


	}
}
