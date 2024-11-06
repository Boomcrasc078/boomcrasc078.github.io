using RaceTimer.Classes;
using System.Buffers.Text;
using System.Collections.Generic;

public class Race
{
    public string Id { get; set; }
    public string Name { get; set; }
    public List<Startlist> Startlists { get; set; } = new List<Startlist>();
    public DateTime creationDateTime { get; set; } = new DateTime();
    public DateTime lastEditDateTime { get; set; } = new DateTime();
    public string currentAnimation { get; set; } = "";
}
