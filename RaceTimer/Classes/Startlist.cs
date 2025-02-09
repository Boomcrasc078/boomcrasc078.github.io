using RaceTimer.Classes;

public class Startlist
{
	public string Id { get; set; } = "";
	public string Name { get; set; } = "";
	public List<Racer> Racers { get; set; } = new List<Racer>();
	public string currentAnimation { get; set; } = "";


	public Startlist DuplicateStartlist()
	{
		Startlist duplicatedStartlist = new Startlist()
		{
			Name = $"Copy of {this.Name}",
			Racers = this.Racers,
		};

		return duplicatedStartlist;
	}
}
