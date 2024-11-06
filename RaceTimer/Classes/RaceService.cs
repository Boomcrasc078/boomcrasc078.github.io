using Microsoft.JSInterop;
using RaceTimer.Classes;
using System.Text.Json;

public class RaceService
{
    private const string LocalStorageKey = "races";
    private readonly IJSRuntime js;

    public RaceService(IJSRuntime js)
    {
        this.js = js;
    }

    public async Task<List<Race>> GetRacesAsync()
    {
        // Hämtar JSON-data från localStorage
        var racesJson = await js.InvokeAsync<string>("localStorage.getItem", LocalStorageKey);

        Console.WriteLine($"Hämtade JSON från localStorage: {racesJson}");

        // Returnerar en tom lista om inget finns sparat i localStorage
        var races = racesJson == null ? new List<Race>() : JsonSerializer.Deserialize<List<Race>>(racesJson, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new DateTimeConverter() } // Konverteraren säkerställer DateTime-parsning
        });

        // Kontrollera och logga datumen
        foreach (var race in races)
        {
            Console.WriteLine($"Race: {race.Name}, Created: {race.creationDateTime}, Last Edited: {race.lastEditDateTime}");
        }

        return races;
    }

    public async Task<Race> GetRaceByIdAsync(string raceId)
    {
        var races = await GetRacesAsync();
        return races.FirstOrDefault(r => r.Id == raceId);
    }

    public async Task SaveRacesAsync(List<Race> races)
    {
        // Sorterar racen efter senaste ändring
        races = races.OrderByDescending(race => race.lastEditDateTime).ToList();

        // Serialiserar listan och sparar den i localStorage
        var racesJson = JsonSerializer.Serialize(races, new JsonSerializerOptions
        {
            Converters = { new DateTimeConverter() }
        });

        Console.WriteLine($"Sparar JSON till localStorage: {racesJson}");

        await js.InvokeVoidAsync("localStorage.setItem", LocalStorageKey, racesJson);
    }

    public async Task UpdateRaceAsync(Race updatedRace)
    {
        var races = await GetRacesAsync();
        var index = races.FindIndex(r => r.Id == updatedRace.Id);

        if (index != -1)
        {
            races[index] = updatedRace;
            await SaveRacesAsync(races);
        }
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

