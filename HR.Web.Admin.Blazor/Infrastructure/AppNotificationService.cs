// Web.Admin.Blazor/Infrastructure/AppNotificationService.cs
using HR.Services.Interfaces;

namespace HR.Web.Admin.Blazor.Infrastructure;

public class AppNotificationService : IAppNotificationService
{
	// This event notifies the UI to refresh counts and play sounds
	public event Action? OnNotificationReceived;

	// This method is called by ANY service (Leave, Advance, etc.)
	public Task NotifyChangeAsync()
	{
		OnNotificationReceived?.Invoke();
		return Task.CompletedTask;
	}
}