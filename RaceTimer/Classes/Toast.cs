using System;

public class Toast
{
	public string id { get; set; }
	public string title { get; set; }
	public string text { get; set; }

	public Toast(string id, string title, string text)
	{
		this.id = id;
		this.title = title;
		this.text = text;
	}
}
