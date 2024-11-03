namespace RaceTimer.Classes
{
	public struct CustomField
	{
		string Name { get; set; }
		string Data { get; set; }

		public CustomField()
		{
			this.Name = "";
			this.Data = "";
		}
	}
	public class Racer
	{


		public string Name { get; set; } = string.Empty;
		public string Surname { get; set; } = string.Empty;
		public string Bib { get; set; } = string.Empty;
		public string Id { get; set; } = string.Empty;
		public DateTime? StartDateTime { get; set; }
		public List<DateTime> LapDateTime { get; set; } = new List<DateTime>();
		public string currentAnimation { get; set; } = string.Empty;
		public List<CustomField> CustomFields { get; set; } = new List<CustomField>();
	}
}
