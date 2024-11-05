using ClosedXML.Excel;
using System.Globalization;
using RaceTimer.Classes;
using System.IO;
using System.Linq;

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
            worksheet.Cell(1, 6).Value = "Startlist";

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

        using (var workbook = new XLWorkbook(fileStream))
        {
            var worksheet = workbook.Worksheet(1);

            foreach (var row in worksheet.RowsUsed().Skip(1))
            {
                string startlistName = row.Cell(6).GetString();
                var startlist = race.Startlists.FirstOrDefault(sl => sl.Name == startlistName) ?? new Startlist { Name = startlistName };

                if (!race.Startlists.Contains(startlist))
                    race.Startlists.Add(startlist);

                var racer = new Racer
                {
                    Name = row.Cell(1).GetString(),
                    Surname = row.Cell(2).GetString(),
                    Bib = row.Cell(3).GetValue<string>(),
                    StartDateTime = DateTime.TryParse($"{row.Cell(4).GetString()} {row.Cell(5).GetString()}", out DateTime startDateTime)
                                    ? startDateTime : (DateTime?)null,
                    Id = Guid.NewGuid().ToString()
                };

                for (int i = 7; i <= row.LastCellUsed().Address.ColumnNumber; i++)
                {
                    var customFieldData = row.Cell(i).GetString();
                    if (!string.IsNullOrEmpty(customFieldData))
                    {
                        racer.CustomFields.Add(new Racer.CustomField(worksheet.FirstRowUsed().Cell(i).GetString(), customFieldData));
                    }
                }

                startlist.Racers.Add(racer);
            }
        }

        return race;
    }
}
