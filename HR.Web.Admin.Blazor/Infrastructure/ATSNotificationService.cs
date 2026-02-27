// HR.Web.Admin.Blazor/Infrastructure/ATSNotificationService.cs
using HR.Services.Interfaces;

namespace HR.Web.Admin.Blazor.Infrastructure;

public class ATSNotificationService : IATSNotificationService
{
	public event Action? OnApplicationReceived;

	public Task NotifyNewApplicationAsync()
	{
		// This fires the event that the Blazor Dashboard will subscribe to
		OnApplicationReceived?.Invoke();
		return Task.CompletedTask;
	}
}