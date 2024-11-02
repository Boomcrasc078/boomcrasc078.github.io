using RaceTimer.Classes;

public class Startlist
{
    public string Id;
    public string Name { get; set; }
    public List<Racer> Racers { get; set; } = new List<Racer>();
    public string currentAnimation = "";
}
