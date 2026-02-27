using HR.Services.Interfaces;

namespace HR.Web.Public.Infrastructure;

public class PublicNotificationService : IAppNotificationService
{
	private readonly IHttpClientFactory _httpClientFactory;
	event Action? IAppNotificationService.OnNotificationReceived
	{
		add { }    
		remove { } 
	}

	public PublicNotificationService(IHttpClientFactory httpClientFactory)
	{
		_httpClientFactory = httpClientFactory;
	}

	public async Task NotifyChangeAsync()
	{
		try
		{
			var client = _httpClientFactory.CreateClient();

			var response = await client.PostAsync("https://localhost:7232/api/notify-ats", null);

			// Ensure we at least log if the bridge failed to reach the Admin
			if (!response.IsSuccessStatusCode)
			{
				Console.WriteLine($"[Notification Bridge]: Admin API returned {response.StatusCode}");
			}
		}
		catch (Exception ex)
		{
			// Log the error - Admin might be down, but the Public app stays stable
			Console.WriteLine($"[Notification Bridge Error]: {ex.Message}");
		}
	}
}