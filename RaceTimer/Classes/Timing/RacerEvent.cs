namespace RaceTimer.Classes.Timing
{
	public class RacerEvent
	{
		public string Name { get; set; }
		public DateTime DateTime { get; set; }
		public string Id { get; private set; }

		// Parameterless constructor for deserialization
		public RacerEvent() { }

		public RacerEvent(string name, IEnumerable<string> existingIds)
		{
			Name = name;
			DateTime = DateTime.Now;
			Id = IdGenerator.GenerateUniqueId(existingIds);
		}

		public RacerEvent(string name, DateTime dateTime, IEnumerable<string> existingIds)
		{
			Name = name;
			DateTime = dateTime;
			Id = IdGenerator.GenerateUniqueId(existingIds);
		}
	}
}
