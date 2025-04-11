using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RaceTimer.Classes;

public static class ExcelHandler
{
	// Exporterar racedata till en Excel-fil och returnerar den som en MemoryStream
	public static MemoryStream ExportRaceToExcel(Race race)
	{
		var memoryStream = new MemoryStream();

		using (var workbook = new XLWorkbook())
		{
			var worksheet = workbook.Worksheets.Add("Race Data");
			int titleRow = 1;
			int dataStartRow = titleRow + 1;

			SetTitleCells(worksheet, titleRow);
			worksheet.Cell(titleRow, 5).Value = "Startlist";

			int startColumn = 7;
			AddCustomFieldsTitles(race.Startlists, worksheet, startColumn);
			ExportRaceData(race, worksheet, dataStartRow);

			workbook.SaveAs(memoryStream);
		}

		memoryStream.Position = 0; // Återställ strömpositionen till början
		return memoryStream;
	}

	// Importerar racedata från en Excel-fil
	public static Race ImportRaceFromExcel(Stream fileStream, List<Race> allRaces, string raceName)
	{
		var race = new Race();

		using (var workbook = new XLWorkbook(fileStream))
		{
			var worksheet = workbook.Worksheet(1);
			var rowsUsed = worksheet.RowsUsed().Skip(1);

			foreach (var row in rowsUsed)
			{
				string startlistName = row.Cell(5).GetString();
				var startlist = GetOrCreateStartlist(race, startlistName);

				var racer = CreateRacerFromRow(row, worksheet, startlist);
				startlist.Racers.Add(racer);
			}
		}

		FinalizeRace(race, allRaces, raceName);
		return race;
	}

	// Exporterar startlistdata till en Excel-fil och returnerar den som en MemoryStream
	public static MemoryStream ExportStartlistToExcel(Startlist startlist)
	{
		var memoryStream = new MemoryStream();

		using (var workbook = new XLWorkbook())
		{
			var worksheet = workbook.Worksheets.Add("Startlist Data");

			int titleRow = 1;
			int dataStartRow = titleRow + 1;
			int customFieldStartColumn = 6;

			SetTitleCells(worksheet, titleRow);
			AddCustomFieldsTitles(new List<Startlist> { startlist }, worksheet, customFieldStartColumn);
			ExportStartlistData(startlist, worksheet, dataStartRow, customFieldStartColumn);

			workbook.SaveAs(memoryStream);
		}

		memoryStream.Position = 0; // Återställ strömpositionen till början
		return memoryStream;
	}

	// Importerar startlistdata från en Excel-fil
	public static Startlist ImportStartlistFromExcel(Stream fileStream, List<Startlist> allStartlists, string startlistName)
	{
		var startlist = new Startlist(startlistName, allStartlists.Select(s => s.Id));

		using (var workbook = new XLWorkbook(fileStream))
		{
			var worksheet = workbook.Worksheet(1);
			var rowsUsed = worksheet.RowsUsed().Skip(1);

			foreach (var row in rowsUsed)
			{
				var racer = CreateRacerFromRow(row, worksheet, startlist);
				startlist.Racers.Add(racer);
			}
		}

		return startlist;
	}

	// Hjälpmetoder

	private static void SetTitleCells(IXLWorksheet worksheet, int row)
	{
		worksheet.Cell(row, 1).Value = "Name";
		worksheet.Cell(row, 2).Value = "Surname";
		worksheet.Cell(row, 3).Value = "Bib";
		worksheet.Cell(row, 4).Value = "Automatic Start";
	}

	private static void AddCustomFieldsTitles(IEnumerable<Startlist> startlists, IXLWorksheet worksheet, int startColumn)
	{
		int col = startColumn;
		var fields = startlists
			.SelectMany(sl => sl.Racers)
			.SelectMany(r => r.CustomFields)
			.Select(cf => cf.Name)
			.Distinct();

		foreach (var field in fields)
		{
			worksheet.Cell(1, col++).Value = field;
		}
	}

	private static void ExportRaceData(Race race, IXLWorksheet worksheet, int startRow)
	{
		int row = startRow;
		foreach (var startlist in race.Startlists)
		{
			foreach (var racer in startlist.Racers)
			{
				ExportRacerData(racer, worksheet, row, startlist.Name);
				row++;
			}
		}
	}

	private static void ExportStartlistData(Startlist startlist, IXLWorksheet worksheet, int startRow, int customFieldStartColumn)
	{
		int row = startRow;
		foreach (var racer in startlist.Racers)
		{
			ExportRacerData(racer, worksheet, row, customFieldStartColumn: customFieldStartColumn);
			row++;
		}
	}

	private static void ExportRacerData(Racer racer, IXLWorksheet worksheet, int row, string startlistName = null, int customFieldStartColumn = 7)
	{
		worksheet.Cell(row, 1).Value = racer.Name;
		worksheet.Cell(row, 2).Value = racer.Surname;
		worksheet.Cell(row, 3).Value = racer.Bib;

		if (racer.StartDateTime != null)
		{
			worksheet.Cell(row, 4).Value = racer.StartDateTime.Value;
		}

		if (startlistName != null)
		{
			worksheet.Cell(row, 5).Value = startlistName;
		}

		int col = customFieldStartColumn;
		foreach (var customField in racer.CustomFields)
		{
			worksheet.Cell(row, col++).Value = customField.Data;
		}
	}

	private static Startlist GetOrCreateStartlist(Race race, string startlistName)
	{
		var startlist = race.Startlists.FirstOrDefault(sl => sl.Name == startlistName) ?? new Startlist(startlistName, race.Startlists.Select(sl => sl.Id));

		if (!race.Startlists.Contains(startlist))
		{
			race.Startlists.Add(startlist);
		}

		return startlist;
	}

	private static Racer CreateRacerFromRow(IXLRow row, IXLWorksheet worksheet, Startlist startlist)
	{
		var racer = new Racer
		{
			Name = row.Cell(1).GetString(),
			Surname = row.Cell(2).GetString(),
			Bib = row.Cell(3).GetValue<string>(),
			StartDateTime = GetDateTimeFromExcel(row.Cell(4)),
			Id = IdGenerator.GenerateUniqueId(startlist.Racers.Select(r => r.Id))
		};

		for (int i = 7; i <= row.LastCellUsed().Address.ColumnNumber; i++)
		{
			var customFieldData = row.Cell(i).GetString();
			if (!string.IsNullOrEmpty(customFieldData))
			{
				racer.CustomFields.Add(new Racer.CustomField(worksheet.RowsUsed().First().Cell(i).GetString(), customFieldData));
			}
		}

		return racer;
	}

	private static void FinalizeRace(Race race, List<Race> allRaces, string raceName)
	{
		race.Startlists = race.Startlists.OrderBy(s => s.Name).ToList();
		race.creationDateTime = DateTime.Now;
		race.lastEditDateTime = DateTime.Now;
		race.Id = IdGenerator.GenerateUniqueId(allRaces.Select(r => r.Id));
		race.Name = raceName;
	}

	private	static DateTime? GetDateTimeFromExcel(IXLCell dateTimeCell)
	{
		if (dateTimeCell.IsEmpty())
		{
			Console.WriteLine("DateTimeCell är tom.");
			return null;
		}

		if (dateTimeCell.DataType != XLDataType.DateTime)
		{
			Console.WriteLine($"DateTimeCell har ogiltig datatyp: {dateTimeCell.DataType}, Värde: {dateTimeCell.Value}");
			return null;
		}

		try
		{
			return dateTimeCell.GetDateTime();
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Fel vid konvertering: {ex.Message}");
			return null;
		}
	}

	public static MemoryStream ExportCustomExcelFile(List<List<string>> data)
	{
		var memoryStream = new MemoryStream();

		using (var workbook = new XLWorkbook())
		{
			var worksheet = workbook.Worksheets.Add("Custom Data");

			for (int rowIndex = 0; rowIndex < data.Count; rowIndex++)
			{
				var row = data[rowIndex];
				for (int colIndex = 0; colIndex < row.Count; colIndex++)
				{
					worksheet.Cell(rowIndex + 1, colIndex + 1).Value = row[colIndex];
				}
			}

			workbook.SaveAs(memoryStream);
		}

		memoryStream.Position = 0; // Återställ strömpositionen till början
		return memoryStream;
	}

}