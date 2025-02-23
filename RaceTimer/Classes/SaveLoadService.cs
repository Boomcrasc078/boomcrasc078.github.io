using System;
using System.IO;
using System.Text.Json;
using Microsoft.JSInterop;
public class SaveLoadService
{
	private readonly IJSRuntime _jsRuntime;

	public SaveLoadService(IJSRuntime jsRuntime)
	{
		_jsRuntime = jsRuntime;
	}

	public async Task SaveAsync<T>(string key, T data)
	{
		var json = JsonSerializer.Serialize(data);
		await _jsRuntime.InvokeVoidAsync("localStorage.setItem", key, json);
	}

	public async Task<T?> LoadAsync<T>(string key)
	{
		var json = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", key);
		if (string.IsNullOrEmpty(json))
		{
			return default;
		}
		var result = JsonSerializer.Deserialize<T>(json);
		return result;
	}
}

