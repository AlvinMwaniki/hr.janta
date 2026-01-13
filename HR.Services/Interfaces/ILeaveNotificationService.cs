// HR.Services/Interfaces/ILeaveNotificationService.cs
namespace HR.Services.Interfaces;

public interface ILeaveNotificationService
{
	event Action<string, string>? OnLeaveUpdated;

	Task NotifyLeaveUpdateAsync(string userId, string message);
}