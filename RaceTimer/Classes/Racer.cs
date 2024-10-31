namespace RaceTimer.Classes
{
	public class Racer
	{
		public string name { get; set; }
		public string surname { get; set; }
		public string bib { get; set; }
		public DateTime startDateTime { get; set; }
		public List<DateTime> lapDateTime { get; set; }
	}
}
