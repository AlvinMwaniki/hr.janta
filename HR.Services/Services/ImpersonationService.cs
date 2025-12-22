// HR.Services/Services/ImpersonationService.cs (UPDATED)


using Microsoft.Extensions.DependencyInjection;
using System;
using HR.Services;

namespace HR.Services.Services;

public class ImpersonationService
{
	// ⭐ NEW: Event to notify subscribers (like MainLayout) ⭐
	public event Action? ImpersonationStateChanged;

	private readonly IServiceProvider _serviceProvider;

	// ⭐ REMOVED: NavigationManager _navigationManager; ⭐
	private bool _isImpersonating = false;

	// ⭐ UPDATED CONSTRUCTOR: Remove NavigationManager parameter ⭐
	public ImpersonationService(IServiceProvider serviceProvider /* , NavigationManager navigationManager - REMOVED */)
	{
		_serviceProvider = serviceProvider;
		// _navigationManager = navigationManager; - REMOVED
	}

	public bool IsImpersonating => _isImpersonating;

	public void ToggleImpersonation()
	{
		_isImpersonating = !_isImpersonating;

		// 1. Notify the Authentication State Provider
		using (var scope = _serviceProvider.CreateScope())
		{
			var authStateProvider = scope.ServiceProvider
				.GetRequiredService<CustomAuthenticationStateProvider>();

			authStateProvider.NotifyStateChange();
		}

		// ⭐ 2. CRITICAL CHANGE: Notify the component (MainLayout) via event ⭐
		ImpersonationStateChanged?.Invoke();

		// ⭐ REMOVED: No navigation here. The component handles it. ⭐
	
	}

	// ⭐ NEW METHOD: Dedicated refresh for when roles/claims are changed by an Admin ⭐
	public void ToggleAuthenticationStateRefresh()
	{
		// The same notification logic used in ToggleImpersonation, but without toggling the flag/navigation
		using (var scope = _serviceProvider.CreateScope())
		{
			var authStateProvider = scope.ServiceProvider
				.GetRequiredService<CustomAuthenticationStateProvider>();

			authStateProvider.NotifyStateChange();
		}
	}

}