using Microsoft.JSInterop;

namespace HR.Services.Services;
public class ThemeService
{
	private readonly IJSRuntime _js;
	public string CurrentMode { get; private set; } = "light"; // Default is Light
	public event Action? OnThemeChanged;
	public ThemeService(IJSRuntime js) => _js = js;

	public async Task SetThemeAsync(string mode)
	{
		CurrentMode = mode;
		await _js.InvokeVoidAsync("localStorage.setItem", "theme", mode);
		// Save to Cookie so the SERVER sees it immediately on next page load
		// This sets a cookie named 'theme' that lasts for 1 year
		await _js.InvokeVoidAsync("eval", $"document.cookie = 'theme={mode}; path=/; max-age=31536000'");
		await _js.InvokeVoidAsync("applyTheme", mode);

		OnThemeChanged?.Invoke();
	}

	public async Task InitializeThemeAsync()
	{
		// Try to get saved preference, default to "light" if none exists
		var saved = await _js.InvokeAsync<string>("localStorage.getItem", "theme");
		CurrentMode = !string.IsNullOrEmpty(saved) ? saved : "light";

		await _js.InvokeVoidAsync("applyTheme", CurrentMode);
	}
}