using RaceTimer.Classes;
using System.Collections.Generic;

public class Race
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<Startlist> Startlists { get; set; } = new List<Startlist>();
}
