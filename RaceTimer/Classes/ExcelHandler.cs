using ClosedXML.Excel;
using System.Globalization;
using RaceTimer.Classes;

public class ExcelHandler
{
    // Export race data to an Excel file and return it as a MemoryStream
    public MemoryStream ExportRaceToExcel(Race race)
    {
        var memoryStream = new MemoryStream();

        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Race Data");
            worksheet.Cell(1, 1).Value = "Name";
            worksheet.Cell(1, 2).Value = "Surname";
            worksheet.Cell(1, 3).Value = "Bib";
            worksheet.Cell(1, 4).Value = "StartDate";
            worksheet.Cell(1, 5).Value = "StartTime";
            worksheet.Cell(1, 6).Value = "Startlist Name";

            // Custom fields dynamically added
            int col = 7;
            foreach (var field in race.Startlists
                .SelectMany(sl => sl.Racers)
                .SelectMany(r => r.CustomFields)
                .Select(cf => cf.Name)
                .Distinct())
            {
                worksheet.Cell(1, col++).Value = field;
            }

            // Populate data for each racer
            int row = 2;
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

                    col = 7;
                    foreach (var customField in racer.CustomFields)
                    {
                        worksheet.Cell(row, col++).Value = customField.Data;
                    }
                    row++;
                }
            }

            workbook.SaveAs(memoryStream);
        }

        memoryStream.Position = 0; // Reset the stream position to the beginning
        return memoryStream;
    }

    // Import race data from an Excel file
    public Race ImportRaceFromExcel(Stream fileStream, string raceName)
    {
        var race = new Race { Name = raceName, creationDateTime = DateTime.UtcNow, lastEditDateTime = DateTime.UtcNow };

        using var workbook = new XLWorkbook(fileStream);
        var worksheet = workbook.Worksheet(1);

        try
        {
            foreach (var row in worksheet.RowsUsed().Skip(1))
            {
                string startlistName;
                try
                {
                    startlistName = row.Cell(6).GetValue<string>();
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Error reading Startlist Name in row {row.RowNumber()}: {ex.Message}");
                }

                var startlist = race.Startlists.FirstOrDefault(sl => sl.Name == startlistName) ?? new Startlist { Name = startlistName };

                if (!race.Startlists.Contains(startlist))
                    race.Startlists.Add(startlist);

                var racer = new Racer();
                try
                {
                    racer.Name = row.Cell(1).GetValue<string>();
                    racer.Surname = row.Cell(2).GetValue<string>();
                    racer.Bib = row.Cell(3).GetValue<string>();  // Läsa Bib som string
                    racer.StartDateTime = ParseDateTime(row.Cell(4).GetValue<string>(), row.Cell(5).GetValue<string>());
                    racer.Id = Guid.NewGuid().ToString();
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Error reading racer data in row {row.RowNumber()}: {ex.Message}");
                }

                // Lägg till Custom Fields
                try
                {
                    for (int i = 7; i <= row.LastCellUsed().Address.ColumnNumber; i++)
                    {
                        var customFieldData = row.Cell(i).GetValue<string>();
                        if (!string.IsNullOrEmpty(customFieldData))
                        {
                            racer.CustomFields.Add(new Racer.CustomField($"CustomField{i - 6}", customFieldData));
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Error reading custom fields in row {row.RowNumber()}: {ex.Message}");
                }

                startlist.Racers.Add(racer);
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error processing Excel file: {ex.Message}");
        }

        return race;
    }

    // Hjälpmetod för att slå ihop datum och tid
    private DateTime? ParseDateTime(string date, string time)
    {
        if (DateTime.TryParse($"{date} {time}", out DateTime parsedDateTime))
        {
            return parsedDateTime;
        }
        return null;
    }
}
