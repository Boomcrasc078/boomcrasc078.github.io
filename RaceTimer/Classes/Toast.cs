using RaceTimer.Classes;
using System;

public class Toast
{
	public string Id { get; set; }
	public string Title { get; set; }
	public string Text { get; set; }
	public DateTime Created { get; set; }
	public string Class { get; set; }
	public Toast(string title, string text)
	{
		this.Id = IdGenerator.GenerateBase64String(5);
		this.Title = title;
		this.Text = text;
		Created = DateTime.Now;
		Class = string.Empty;
	}
	public Toast(string title, string text, string @class)
	{
		this.Id = IdGenerator.GenerateBase64String(5);
		this.Title = title;
		this.Text = text;
		Created = DateTime.Now;
		Class = @class;
	}

	public Toast(string title, string text, string @class, DateTime created)
	{
		this.Id = IdGenerator.GenerateBase64String(5);
		this.Title = title;
		this.Text = text;
		this.Class = @class;
		this.Created = created;
	}
}
