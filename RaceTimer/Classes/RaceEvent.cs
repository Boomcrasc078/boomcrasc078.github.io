using RaceTimer.Classes;
using System;

public class RaceEvent
{
	public Race Race { get; }
	public DateTime DateTime { get; }
	public Racer Racer { get; }
	public string Type { get; }
	public string Message { get; }

	public RaceEvent(Race race, Racer racer, string type)
	{
		Race = race;
		Racer = racer;
		Type = type;

		this.DateTime = DateTime.Now;

		switch (type)
		{
			case "finish":
				Message = $"{racer.Name} {racer.Surname} finished lap {Racer.LapDateTime.Count} at {this.DateTime.ToString("HH:mm:ss.ff")}.";
				break;

			default:
				Message = $"An event without type happened with {Racer.Name} {Racer.Surname} at {this.DateTime.ToString("HH:mm:ss.ff")}.";
				break;
		}
	}
}
