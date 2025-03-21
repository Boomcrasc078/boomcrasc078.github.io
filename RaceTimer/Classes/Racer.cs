﻿namespace RaceTimer.Classes
{

	public class Racer
	{
		public string Name { get; set; } = string.Empty;
		public string Surname { get; set; } = string.Empty;
		public string Bib { get; set; } = string.Empty;
		public string Id { get; set; } = string.Empty;
		public DateTime? StartDateTime { get; set; } = null;
		public List<DateTime> LapDateTime { get; set; } = new List<DateTime>();
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

		string? GetTime()
		{
			if (LapDateTime.Count < 0)
				return null;
			if (StartDateTime == null)
				return null;
			var time = LapDateTime.Last() - StartDateTime.Value;
			var timeString = time.ToString("hh\\:mm\\:ss\\.ff");
			return timeString;
		}

		string? GetPace(float distanceMeters)
		{
			if (LapDateTime.Count < 0)
				return null;
			if (StartDateTime == null)
				return null;
			var time = LapDateTime.Last() - StartDateTime.Value;
			var pace = time / (distanceMeters / 1000);

			var stringPace = pace.ToString("mm//:ss");
			return stringPace;
		}

		string? GetSpeed(float distanceMeters)
		{
			if (LapDateTime.Count < 0)
				return null;
			if (StartDateTime == null)
				return null;
			var time = LapDateTime.Last() - StartDateTime.Value;
			var speed = (distanceMeters / 1000) / time.TotalHours;

			var stringSpeed = speed.ToString();
			return stringSpeed;
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
