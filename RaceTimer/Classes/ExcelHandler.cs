using ClosedXML.Excel;
using System.Globalization;
using RaceTimer.Classes;
using System.IO;
using System.Linq;
using DocumentFormat.OpenXml.Spreadsheet;
using RaceTimer.Pages;

public class ExcelHandler
{

	void RaceExportData(Race race, IXLWorksheet worksheet, int startRow)
	{
		int row = startRow;
		foreach (var startlist in race.Startlists)
		{
			foreach (var racer in startlist.Racers)
			{
				worksheet.Cell(row, 1).Value = racer.Name;
				worksheet.Cell(row, 2).Value = racer.Surname;
				worksheet.Cell(row, 3).Value = racer.Bib;
				worksheet.Cell(row, 4).Value = racer.StartDateTime?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
				worksheet.Cell(row, 5).Value = racer.StartDateTime?.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
				worksheet.Cell(row, 6).Value = startlist.Name;

				int col = 7;
				foreach (var customField in racer.CustomFields)
				{
					worksheet.Cell(row, col++).Value = customField.Data;
				}
				row++;
			}
		}
	}

	void StartlistExportData(Startlist startlist, IXLWorksheet worksheet, int startRow, int customFieldStartColumn)
	{
		int row = startRow;

		foreach (var racer in startlist.Racers)
		{
			worksheet.Cell(row, 1).Value = racer.Name;
			worksheet.Cell(row, 2).Value = racer.Surname;
			worksheet.Cell(row, 3).Value = racer.Bib;
			worksheet.Cell(row, 4).Value = racer.StartDateTime?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
			worksheet.Cell(row, 5).Value = racer.StartDateTime?.ToString("HH:mm:ss", CultureInfo.InvariantCulture);

			// Custom fields dynamically added
			int col = customFieldStartColumn;
			foreach (var customField in racer.CustomFields)
			{
				worksheet.Cell(row, col++).Value = customField.Data;
			}
			row++;
		}
	}

	void SetTitleCells(IXLWorksheet worksheet, int row)
	{
		worksheet.Cell(row, 1).Value = "Name";
		worksheet.Cell(row, 2).Value = "Surname";
		worksheet.Cell(row, 3).Value = "Bib";
		worksheet.Cell(row, 4).Value = "StartDate";
		worksheet.Cell(row, 5).Value = "StartTime";
	}

	void AddCustomFieldsTitle(Startlist startlist, IXLWorksheet worksheet, int startColumn)
	{
		int col = startColumn;
		foreach (var field in startlist.Racers
			.SelectMany(r => r.CustomFields)
			.Select(cf => cf.Name)
			.Distinct())
		{
			worksheet.Cell(1, col++).Value = field;
		}
	}

	// Export race data to an Excel file and return it as a MemoryStream
	public MemoryStream ExportRaceToExcel(Race race)
	{
		var memoryStream = new MemoryStream();

		using (var workbook = new XLWorkbook())
		{
			var worksheet = workbook.Worksheets.Add("Race Data");

			int titleRow = 1;
			int dataStartRow = titleRow + 1;

			SetTitleCells(worksheet, titleRow);

			worksheet.Cell(titleRow, 6).Value = "Startlist";


			int startColumn = 7;
			// Custom fields dynamically added
			foreach (var startlist in race.Startlists)
			{
				AddCustomFieldsTitle(startlist, worksheet, startColumn);
			}

			RaceExportData(race, worksheet, dataStartRow);

			workbook.SaveAs(memoryStream);
		}

		memoryStream.Position = 0; // Reset the stream position to the beginning
		return memoryStream;
	}

	// Import race data from an Excel file
	public Race ImportRaceFromExcel(Stream fileStream, List<Race> allRaces, string raceName)
	{
		var race = new Race();

		using (var workbook = new XLWorkbook(fileStream))
		{
			var worksheet = workbook.Worksheet(1);
			var rowsUsed = worksheet.RowsUsed().Skip(1);

			foreach (var row in rowsUsed)
			{
				string startlistName = row.Cell(6).GetString();
				var startlist = race.Startlists.FirstOrDefault(sl => sl.Name == startlistName) ?? new Startlist { Name = startlistName };

				if (!race.Startlists.Contains(startlist))
				{
					Console.WriteLine($"Giving startlist {startlist.Name} a new Id...");
					startlist.Id = IdGenerator.GenerateBase64String(5);
					while (race.Startlists.Exists(sl => sl.Id == startlist.Id) || startlist.Id.Contains("/"))
					{
						startlist.Id = IdGenerator.GenerateBase64String(5);
					}

					race.Startlists.Add(startlist);
				}
				Console.WriteLine($"Startlist {startlist.Name} got {startlist.Id} as Id");
				var racer = new Racer
				{
					Name = row.Cell(1).GetString(),
					Surname = row.Cell(2).GetString(),
					Bib = row.Cell(3).GetValue<string>(),
					StartDateTime = DateTime.TryParse($"{row.Cell(4).GetString()} {row.Cell(5).GetString()}", out DateTime startDateTime)
									? startDateTime : (DateTime?)null,
					Id = IdGenerator.GenerateBase64String(5),
				};

				while (race.Startlists.SelectMany(s => s.Racers).Select(racer => racer.Id).Contains(racer.Id) || racer.Id.Contains("/"))
				{
					racer.Id = IdGenerator.GenerateBase64String(5);
				}

				for (int i = 7; i <= row.LastCellUsed().Address.ColumnNumber; i++)
				{
					var customFieldData = row.Cell(i).GetString();
					if (!string.IsNullOrEmpty(customFieldData))
					{
						racer.CustomFields.Add(new Racer.CustomField(worksheet.RowsUsed().First().Cell(i).GetString(), customFieldData));
					}
				}

				startlist.Racers.Add(racer);
			}
		}

		race.Id = IdGenerator.GenerateBase64String(5);
		while (allRaces.Exists(r => r.Id == race.Id) || race.Id.Contains("/"))
		{
			race.Id = IdGenerator.GenerateBase64String(5);
		}

		race.Startlists = race.Startlists.OrderBy(startlist => startlist.Name).ToList();
		race.creationDateTime = DateTime.Now;
		race.lastEditDateTime = DateTime.Now;

		race.Name = raceName;

		return race;
	}





	public MemoryStream ExportStartlistToExcel(Startlist startlist)
	{
		var memoryStream = new MemoryStream();

		using (var workbook = new XLWorkbook())
		{
			var worksheet = workbook.Worksheets.Add("Startlist Data");

			int titleRow = 1;
			int dataStartRow = titleRow + 1;

			int customFieldStartColumn = 6;

			SetTitleCells(worksheet, titleRow);

			AddCustomFieldsTitle(startlist, worksheet, customFieldStartColumn);

			StartlistExportData(startlist, worksheet, dataStartRow, customFieldStartColumn);

			workbook.SaveAs(memoryStream);
		}
		memoryStream.Position = 0; // Reset the stream position to the beginning
		return memoryStream;
	}

	public Startlist ImportStartlistFromExcel(Stream fileStream, List<Startlist> allStartlists, string startlistName)
	{
		var startlist = new Startlist();

		using (var workbook = new XLWorkbook(fileStream))
		{
			var worksheet = workbook.Worksheet(1);
			var rowsUsed = worksheet.RowsUsed().Skip(1);

			startlist.Name = startlistName;
			startlist.Id = IdGenerator.GenerateBase64String(5);
			while (allStartlists.Exists(r => r.Id == startlist.Id) || startlist.Id.Contains("/"))
			{
				startlist.Id = IdGenerator.GenerateBase64String(5);
			}

			foreach (var row in rowsUsed)
			{
				var racer = new Racer
				{
					Name = row.Cell(1).GetString(),
					Surname = row.Cell(2).GetString(),
					Bib = row.Cell(3).GetValue<string>(),
					StartDateTime = DateTime.TryParse($"{row.Cell(4).GetString()} {row.Cell(5).GetString()}", out DateTime startDateTime)
									? startDateTime : (DateTime?)null,
					Id = IdGenerator.GenerateBase64String(5),
				};

				while (startlist.Racers.Select(racer => racer.Id).Contains(racer.Id) || racer.Id.Contains("/"))
				{
					racer.Id = IdGenerator.GenerateBase64String(5);
				}

				for (int i = 6; i <= row.LastCellUsed().Address.ColumnNumber; i++)
				{
					var customFieldData = row.Cell(i).GetString();
					if (!string.IsNullOrEmpty(customFieldData))
					{
						racer.CustomFields.Add(new Racer.CustomField(worksheet.RowsUsed().First().Cell(i).GetString(), customFieldData));
					}
				}

				startlist.Racers.Add(racer);
			}
		}
		return startlist;
	}
}
