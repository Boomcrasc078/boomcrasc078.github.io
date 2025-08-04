namespace RaceTimer.Classes.Timing
{
	public class RaceType
	{
		public string Name { get; set; }
		public string Id { get; set; }
		public RaceType(string name, string id)
		{
			Name = name;
			Id = id;
		}
		public static RaceType SingleCourse => new RaceType("Single Course", "SingleCourse");
		public static RaceType MultipleLaps => new RaceType("Multiple Laps", "MultipleLaps");
		public static RaceType TimeBased => new RaceType("Time Based", "TimeBased");


		public static List<RaceType> RaceTypes { get; } = new List<RaceType> {SingleCourse, MultipleLaps, TimeBased};
	}
}
