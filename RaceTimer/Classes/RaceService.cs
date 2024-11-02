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
        var racesJson = await js.InvokeAsync<string>("localStorage.getItem", LocalStorageKey);
        return racesJson == null ? new List<Race>() : JsonSerializer.Deserialize<List<Race>>(racesJson);
    }

    public async Task<Race> GetRaceByIdAsync(string raceId)
    {
        var races = await GetRacesAsync();
        return races.FirstOrDefault(r => r.Id == raceId);
    }

    public async Task SaveRacesAsync(List<Race> races)
    {
        //races = races.OrderByDescending(race => race.lastEditDateTime).ToList();
        var racesJson = JsonSerializer.Serialize(races);
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
