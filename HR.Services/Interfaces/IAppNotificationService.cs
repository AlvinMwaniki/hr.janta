namespace HR.Services.Interfaces;

public interface IAppNotificationService
{
	event Action? OnNotificationReceived; 
	Task NotifyChangeAsync();
}