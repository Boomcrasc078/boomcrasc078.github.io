﻿using RaceTimer.Classes;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

public class Startlist
{
	public string Id { get; set; }
	public string Name { get; set; }
	public List<Racer> Racers { get; set; } = new();
	public string CurrentAnimation { get; set; } = string.Empty;
	public float DistanceMeters { get; set; }
	public string StartType { get; set; }
	public Startlist() { }
	public Startlist(string name, IEnumerable<string> existingIds)
	{
		Name = name;
		Id = IdGenerator.GenerateUniqueId(existingIds);
		Racers = new();
		DistanceMeters = 0;
		StartType = "mass-start";
	}

	public Startlist DuplicateStartlist(IEnumerable<string> existingIds)
	{
		Startlist duplicatedStartlist = new Startlist($"Copy of {this.Name}", existingIds)
		{
			Racers = this.Racers,
		};

		return duplicatedStartlist;
	}

	public DateTime? FirstStartDateTime()
	{
		DateTime? firstDateTime = null;

		foreach (var racer in Racers)
		{
			if (firstDateTime == null || racer.StartDateTime < firstDateTime)
			{
				firstDateTime = racer.StartDateTime;
			}
		}

		return firstDateTime;
	}

	public List<DateTime> AllStartDateTime()
	{
		List<DateTime> DateTimes = new List<DateTime>();

		foreach (var racer in Racers)
		{
			if(racer.StartDateTime == null)
			{
				continue;
			}

			if (!DateTimes.Contains(racer.StartDateTime.Value))
			{
				Console.WriteLine("Startlist: " + racer.StartDateTime.Value);
				DateTimes.Add(racer.StartDateTime.Value);
			}
		}

		DateTimes = DateTimes.Order().ToList<DateTime>();

		return DateTimes;
	}

	public static string StartDatetimeString(DateTime? startDateTime)
	{
		if (!startDateTime.HasValue)
		{
			return "Not Set";
		}

		string dateTimeString = startDateTime.Value.ToString("HH:mm - yyyy-MM-dd");

		return dateTimeString;
	}
}
