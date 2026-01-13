using HR.Services.Interfaces;

namespace HR.Web.Admin.Blazor.Infrastructure;

public class LeaveNotificationService : ILeaveNotificationService
{
	// This is our "Internal SignalR"
	public event Action<string, string>? OnLeaveUpdated;

	public Task NotifyLeaveUpdateAsync(string userId, string message)
	{
		// ina trigger the C# event
		OnLeaveUpdated?.Invoke(userId, message);
		return Task.CompletedTask;
	}
}