using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using RaceTimer.Classes;

public class CsvHandler
{
	// Exporterar en tävling till en CSV-fil
	public MemoryStream ExportRaceToCsv(Race race)
	{
		var memoryStream = new MemoryStream();
		using (var writer = new StreamWriter(memoryStream))
		{
			// Skriv rubriker
			var header = "Name,Surname,Bib,StartDate,StartTime,Startlist";
			var customFields = race.Startlists
				.SelectMany(sl => sl.Racers.SelectMany(r => r.CustomFields.Select(cf => cf.Name)))
				.Distinct();
			header += "," + string.Join(",", customFields);
			writer.WriteLine(header);

			// Skriv racerdata
			foreach (var startlist in race.Startlists)
			{
				foreach (var racer in startlist.Racers)
				{
					var line = $"{racer.Name},{racer.Surname},{racer.Bib}," +
							   $"{racer.StartDateTime?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}," +
							   $"{racer.StartDateTime?.ToString("HH:mm:ss", CultureInfo.InvariantCulture)}," +
							   $"{startlist.Name}";

					var customFieldData = customFields
						.Select(cfName => racer.CustomFields.FirstOrDefault(cf => cf.Name == cfName)?.Data ?? "");
					line += "," + string.Join(",", customFieldData);

					writer.WriteLine(line);
				}
			}
		}
		memoryStream.Position = 0; // Reset stream-position
		return memoryStream;
	}

	// Importerar en tävling från en CSV-fil
	public Race ImportRaceFromCsv(Stream fileStream, List<Race> allRaces, string raceName)
	{
		var race = new Race { Name = raceName, Startlists = new List<Startlist>() };
		using (var reader = new StreamReader(fileStream))
		{
			var header = reader.ReadLine()?.Split(',') ?? throw new InvalidDataException("Filen är tom eller saknar rubriker.");
			var customFieldStartIndex = Array.IndexOf(header, "Startlist") + 1;

			string line;
			while ((line = reader.ReadLine()) != null)
			{
				var columns = line.Split(',');

				var startlistName = columns[5];
				var startlist = race.Startlists.FirstOrDefault(sl => sl.Name == startlistName) ?? new Startlist { Name = startlistName, Racers = new List<Racer>() };

				if (!race.Startlists.Contains(startlist))
				{
					startlist.Id = GenerateUniqueId(allRaces.SelectMany(r => r.Startlists).Select(sl => sl.Id));
					race.Startlists.Add(startlist);
				}

				var racer = new Racer
				{
					Name = columns[0],
					Surname = columns[1],
					Bib = columns[2],
					StartDateTime = DateTime.TryParse($"{columns[3]} {columns[4]}", out var dateTime) ? dateTime : (DateTime?)null,
					Id = GenerateUniqueId(startlist.Racers.Select(r => r.Id)),
					CustomFields = new List<Racer.CustomField>()
				};

				for (int i = customFieldStartIndex; i < columns.Length; i++)
				{
					if (!string.IsNullOrEmpty(columns[i]))
					{
						racer.CustomFields.Add(new Racer.CustomField(header[i], columns[i]));
					}
				}

				startlist.Racers.Add(racer);
			}
		}

		race.Id = GenerateUniqueId(allRaces.Select(r => r.Id));
		race.creationDateTime = DateTime.Now;
		race.lastEditDateTime = DateTime.Now;

		return race;
	}

	// Exporterar en startlista till en CSV-fil
	public MemoryStream ExportStartlistToCsv(Startlist startlist)
	{
		var memoryStream = new MemoryStream();
		using (var writer = new StreamWriter(memoryStream))
		{
			var header = "Name,Surname,Bib,StartDate,StartTime";
			var customFields = startlist.Racers
				.SelectMany(r => r.CustomFields.Select(cf => cf.Name))
				.Distinct();
			header += "," + string.Join(",", customFields);
			writer.WriteLine(header);

			foreach (var racer in startlist.Racers)
			{
				var line = $"{racer.Name},{racer.Surname},{racer.Bib}," +
						   $"{racer.StartDateTime?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}," +
						   $"{racer.StartDateTime?.ToString("HH:mm:ss", CultureInfo.InvariantCulture)}";

				var customFieldData = customFields
					.Select(cfName => racer.CustomFields.FirstOrDefault(cf => cf.Name == cfName)?.Data ?? "");
				line += "," + string.Join(",", customFieldData);

				writer.WriteLine(line);
			}
		}
		memoryStream.Position = 0;
		return memoryStream;
	}

	// Importerar en startlista från en CSV-fil
	public Startlist ImportStartlistFromCsv(Stream fileStream, List<Startlist> allStartlists, string startlistName)
	{
		var startlist = new Startlist { Name = startlistName, Racers = new List<Racer>() };

		using (var reader = new StreamReader(fileStream))
		{
			var header = reader.ReadLine()?.Split(',') ?? throw new InvalidDataException("Filen är tom eller saknar rubriker.");
			int customFieldStartIndex = 5;

			string line;
			while ((line = reader.ReadLine()) != null)
			{
				var columns = line.Split(',');

				var racer = new Racer
				{
					Name = columns[0],
					Surname = columns[1],
					Bib = columns[2],
					StartDateTime = DateTime.TryParse($"{columns[3]} {columns[4]}", out var dateTime) ? dateTime : (DateTime?)null,
					Id = GenerateUniqueId(startlist.Racers.Select(r => r.Id)),
					CustomFields = new List<Racer.CustomField>()
				};

				for (int i = customFieldStartIndex; i < columns.Length; i++)
				{
					if (!string.IsNullOrEmpty(columns[i]))
					{
						racer.CustomFields.Add(new Racer.CustomField(header[i], columns[i]));
					}
				}

				startlist.Racers.Add(racer);
			}
		}

		startlist.Id = GenerateUniqueId(allStartlists.Select(sl => sl.Id));
		return startlist;
	}

	private string GenerateUniqueId(IEnumerable<string> existingIds)
	{
		string id;
		do
		{
			id = IdGenerator.GenerateBase64String(5);
		} while (existingIds.Contains(id) || id.Contains("/"));
		return id;
	}
}
