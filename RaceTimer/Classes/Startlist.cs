using DocumentFormat.OpenXml.Office2021.DocumentTasks;
using Microsoft.AspNetCore.Components;
using RaceTimer.Classes;
using RaceTimer.Classes.Timing;
using RaceTimer.Components;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

public class Startlist
{
	public string Id { get; set; }
	public string Name { get; set; }
	public List<Racer> Racers { get; set; } = new();
	public string CurrentAnimation { get; set; } = string.Empty;
	public float DistanceKm { get; set; }
	public StartType StartType { get; set; }
	public RaceType RaceType { get; set; }
	public int LapCount { get; set; }
	public Startlist() { }
	public Startlist(string name, IEnumerable<string> existingIds)
	{
		Name = name;
		Id = IdGenerator.GenerateUniqueId(existingIds);
		Racers = new();
		DistanceKm = 0;
		StartType = StartType.MassStart;
		RaceType = RaceType.SingleCourse;
		LapCount = 1;
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

	public void InputRaceType(ChangeEventArgs args)
	{
		var value = args.Value;
		if (value == null)
		{
			return;
		}
		var stringValue = value.ToString();
		if (stringValue == null)
		{
			return;
		}
		var getSRacetype = RaceType.RaceTypes.First(x => x.Id == stringValue);
		this.RaceType = getSRacetype;

		if (this.RaceType.Id == RaceType.SingleCourse.Id)
		{
			this.LapCount = 1;
		}
	}

	public void InputStarttype(ChangeEventArgs args)
	{
		var value = args.Value;
		if (value == null)
		{
			return;
		}
		var stringValue = value.ToString();
		if (stringValue == null)
		{
			return;
		}
		var getStarttype = StartType.StartTypes.First(x => x.Id == stringValue);
		StartType = getStarttype;
	}


	public async System.Threading.Tasks.Task InputDistance(ChangeEventArgs eventArgs, Toasts toasts)
	{
		var value = eventArgs.Value;
		if (value == null)
		{
			return;
		}

		try
		{
			float floatValue = float.Parse(value.ToString() ?? "0", CultureInfo.InvariantCulture);
			DistanceKm = floatValue;
		}
		catch (Exception exception)
		{
			await toasts.CreateToast(new Toast("", exception.Message, "text-bg-danger"));
		}
	}

	public async System.Threading.Tasks.Task InputLapCount(ChangeEventArgs args, Toasts toasts)
	{
		var value = args.Value;
		if (value == null)
		{
			return;
		}

		try
		{
			float floatValue = float.Parse(value.ToString() ?? "1", CultureInfo.InvariantCulture);
			int intValue = (int)MathF.Floor(floatValue);
			LapCount = intValue;
		}
		catch (Exception exception)
		{
			await toasts.CreateToast(new Toast("", exception.Message, "text-bg-danger"));
		}
	}
}
