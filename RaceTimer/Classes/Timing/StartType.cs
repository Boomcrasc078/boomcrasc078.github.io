namespace RaceTimer.Classes.Timing
{
	public class StartType
	{
		public string Name { get; set; }
		public string Id { get; set; }
		public StartType(string name, string id)
		{
			Name = name;
			Id = id;
		}
		public static StartType MassStart => new StartType("Mass Start", "MassStart");
		public static StartType IndividualStart => new StartType("Individual Start", "IndividualStart");
		public static List<StartType> StartTypes { get; } = new List<StartType> { MassStart, IndividualStart };
	}
}
