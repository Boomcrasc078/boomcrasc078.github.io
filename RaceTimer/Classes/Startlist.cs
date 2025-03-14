using RaceTimer.Classes;

public class Startlist
{
	public string Id { get; set; } = string.Empty;
	public string Name { get; set; } = string.Empty;
	public List<Racer> Racers { get; set; } = new List<Racer>();
	public string CurrentAnimation { get; set; } = string.Empty;
	public float Distance { get; set; }

	public Startlist DuplicateStartlist(IEnumerable<string> existingIds)
	{
		Startlist duplicatedStartlist = new Startlist()
		{
			Name = $"Copy of {this.Name}",
			Racers = this.Racers,
			Id = IdGenerator.GenerateUniqueId(existingIds)
		};

		return duplicatedStartlist;
	}
}
