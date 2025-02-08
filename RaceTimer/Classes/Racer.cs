namespace RaceTimer.Classes
{

	public class Racer
	{
		public string Name { get; set; } = string.Empty;
		public string Surname { get; set; } = string.Empty;
		public string Bib { get; set; } = string.Empty;
		public string Id { get; set; } = string.Empty;
		public DateTime? StartDateTime { get; set; } = null;
		public List<DateTime> LapDateTime { get; set; } = new List<DateTime>();
		public string currentAnimation { get; set; } = string.Empty;
		public List<CustomField> CustomFields { get; set; } = new List<CustomField>();
		public class CustomField
		{
			public string Name { get; set; }
			public string Data { get; set; }

			public CustomField(string name, string data)
			{
				Name = name;
				Data = data;
			}
		}

	}
}
