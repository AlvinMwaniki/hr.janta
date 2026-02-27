using Microsoft.AspNetCore.SignalR;

namespace HR.Services.Services;

public class RefreshBroker
{

	public event Func<Task>? OnRequestChanged;

	public event Func<string, string, Task>? OnNewRequestReceived;

	public async Task CallRequestChanged()
	{
		if (OnRequestChanged != null)
		{
			await OnRequestChanged.Invoke();
		}
	}

	public async Task NotifyNewRequest(string type, string name)
	{
		if (OnNewRequestReceived != null)
		{
			await OnNewRequestReceived.Invoke(type, name);
		}
		await CallRequestChanged();
	}
}