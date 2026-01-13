// HR.Services/Services/ImpersonationService.cs (UPDATED)


using Microsoft.Extensions.DependencyInjection;
using System;
using HR.Services;

namespace HR.Services.Services;

public class ImpersonationService
{
	public bool IsInEmployeeView { get; private set; }
	public bool IsImpersonating { get; private set; } // Your existing flag

	public event Action? OnStateChanged;

	public void ToggleViewMode()
	{
		IsInEmployeeView = !IsInEmployeeView;
		OnStateChanged?.Invoke();
	}

	private void NotifyStateChanged() => OnStateChanged?.Invoke();

	// Your existing refresh logic
	public void ToggleAuthenticationStateRefresh() => NotifyStateChanged();
}