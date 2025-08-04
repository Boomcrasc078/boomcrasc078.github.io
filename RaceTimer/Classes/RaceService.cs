using Microsoft.JSInterop;
using RaceTimer.Classes;
using System.Text.Json;

public static class RaceService
{
	private const string LocalStorageKey = "races";

	public static async Task<List<Race>> GetRacesAsync(IJSRuntime js)
	{
		// Hämtar JSON-data från localStorage
		var racesJson = await js.InvokeAsync<string>("localStorage.getItem", LocalStorageKey);

		// Returnerar en tom lista om inget finns sparat i localStorage
		var races = racesJson == null ? new List<Race>() : JsonSerializer.Deserialize<List<Race>>(racesJson, new JsonSerializerOptions
		{
			PropertyNameCaseInsensitive = true,
			Converters = { new DateTimeConverter() } // Konverteraren säkerställer DateTime-parsning
		}) ?? new List<Race>(); // Ensure races is not null

		// Kontrollera och logga datumen
		if (races != null)
		{
			foreach (var race in races)
			{
				Console.WriteLine($"Race: {race.Name}, Created: {race.creationDateTime}, Last Edited: {race.lastEditDateTime}");
			}
		}

		return races ?? new List<Race>();
	}

	public static async Task<Race?> GetRaceByIdAsync(string raceId, IJSRuntime js)
	{
		var races = await GetRacesAsync(js);

		if(races == null)
		{
			return null;
		}

		return races.FirstOrDefault(r => r.Id == raceId);
	}

	public static async Task SaveRacesAsync(List<Race> races, IJSRuntime js)
	{
		// Sorterar racen efter senaste ändring
		races = races.OrderByDescending(race => race.lastEditDateTime).ToList();

		// Serialiserar listan och sparar den i localStorage
		var racesJson = JsonSerializer.Serialize(races, new JsonSerializerOptions
		{
			Converters = { new DateTimeConverter() }
		});

		Console.WriteLine($"Sparar alla race till localStorage");

		await js.InvokeVoidAsync("localStorage.setItem", LocalStorageKey, racesJson);
	}

    public static async Task UpdateRaceAsync(Race updatedRace, IJSRuntime js)
    {
        updatedRace.Startlists = updatedRace.Startlists
            .OrderBy(s => ExtractNumericPrefix(s.Name)) // Sort numerically by prefix
            .ThenBy(s => s.Name) // Then sort alphabetically
            .ToList();

		updatedRace.lastEditDateTime = DateTime.Now;

        var races = await GetRacesAsync(js);
        var index = races.FindIndex(r => r.Id == updatedRace.Id);

        if (index == -1)
        {
            return;
        }

        races[index] = updatedRace;
        await SaveRacesAsync(races, js);
    }

    // Helper method to extract numeric prefix
    private static int ExtractNumericPrefix(string name)
    {
        var match = System.Text.RegularExpressions.Regex.Match(name, @"^\d+");
        return match.Success ? int.Parse(match.Value) : int.MaxValue; // Use int.MaxValue for non-numeric names
    }
}

// DateTime-konverterare för att säkerställa korrekt serialisering/deserialisering av DateTime
public class DateTimeConverter : System.Text.Json.Serialization.JsonConverter<DateTime>
{
	public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		var dateStr = reader.GetString();
		if (string.IsNullOrWhiteSpace(dateStr)) // Kolla om strängen är tom
		{
			Console.WriteLine("Datumsträngen var tom eller null");
			return DateTime.MinValue;
		}

		if (DateTime.TryParse(dateStr, null, System.Globalization.DateTimeStyles.RoundtripKind, out var date))
		{
			return date;
		}
		else
		{
			Console.WriteLine($"Ogiltigt datumformat: {dateStr}");
			return DateTime.MinValue;
		}
	}

	public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
	{
		writer.WriteStringValue(value.ToString("o")); // ISO 8601-format
	}
}

