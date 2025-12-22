// HR.Services/Services/CustomAuthenticationStateProvider.cs

using HR.Data;
using HR.Services.Constants; // ⭐ NEW: Needed for AppRoles and AppPermissions ⭐
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

using System.Linq; // ⭐ NEW: Needed for .Where() and .ToList() ⭐
using System.Security.Claims;
using System.Threading.Tasks;

namespace HR.Services.Services;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly ImpersonationService _impersonationService; // ⭐ NEW FIELD ⭐
	private readonly IServiceProvider _serviceProvider;
	private readonly IServiceScopeFactory _serviceScopeFactory;
	private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());
	private readonly IEmployeeDataCacheService _cacheService;

	// ⭐ UPDATED CONSTRUCTOR: Now injects ImpersonationService ⭐
	public CustomAuthenticationStateProvider(IHttpContextAccessor httpContextAccessor,
											 ImpersonationService impersonationService,
											 IServiceProvider serviceProvider, IEmployeeDataCacheService cacheService, 
											 IServiceScopeFactory serviceScopeFactory)
	{
		_httpContextAccessor = httpContextAccessor;
		_impersonationService = impersonationService;
		_serviceProvider = serviceProvider;
		_cacheService = cacheService;
		_serviceScopeFactory = serviceScopeFactory;
	}

	public override async Task<AuthenticationState> GetAuthenticationStateAsync()
	{
		if (_httpContextAccessor.HttpContext?.User is null)
		{
			return new AuthenticationState(_anonymous);
		}

		var result = await _httpContextAccessor.HttpContext.AuthenticateAsync("CustomAuth");

		if (result?.Principal is not null && result.Succeeded)
		{
			var principalFromCookie = result.Principal;

			// 1. Get the user's permanent ID from the static cookie claims
			if (principalFromCookie.Identity is not ClaimsIdentity baseIdentity)
			{
				// If the identity is somehow missing or not a ClaimsIdentity, fall back to the basic principal
				return new AuthenticationState(principalFromCookie);
			}

			// Get the user's permanent ID from the static cookie claims
			var userIdClaim = principalFromCookie.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
			if (userIdClaim is null || !Guid.TryParse(userIdClaim.Value, out var userId))
			{
				return new AuthenticationState(principalFromCookie);
			}

			// 1. Fetch all claims (static claims + dynamic DB permissions), passing the safely casted identity
			var finalClaims = await GetFullClaimsSet(userId, baseIdentity);
			var fullPrincipal = new ClaimsPrincipal(new ClaimsIdentity(finalClaims, "CustomAuth"));

			// 3. ⭐ CHECK IMPERSONATION STATE AND APPLY FILTERING ⭐
			if (_impersonationService.IsImpersonating)
			{
				// 3a. Get only the identity claims (NameId, Name, Email)
				var impersonatedClaims = fullPrincipal.Claims
					.Where(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier ||
								c.Type == System.Security.Claims.ClaimTypes.Name ||
								c.Type == System.Security.Claims.ClaimTypes.Email ||

                                c.Type == "EmployeeId")
					.ToList();

				// 3b. Add the Employee role claim (to satisfy @AppRoles.Employee checks)
				impersonatedClaims.Add(new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, AppRoles.Employee));

				// 3c. Manually add the base permissions needed for the employee view
				// CRITICAL: Load all permissions associated with the Employee Role ID to mimic the employee.
				// To do this cleanly, we need to access the database again inside this block.

				using (var scope = _serviceProvider.CreateScope())
				{
					var dbContext = scope.ServiceProvider.GetRequiredService<HR.Data.HRDbContext>();
					var employeeRole = await dbContext.Roles.AsNoTracking().FirstOrDefaultAsync(r => r.Name == AppRoles.Employee);

					if (employeeRole != null)
					{
						var employeeRolePermissions = await dbContext.RolePermissions
							.Where(rp => rp.RoleId == employeeRole.Id)
							.Select(rp => rp.PermissionCode)
							.ToListAsync();

						// Add the permission claims
						impersonatedClaims.AddRange(employeeRolePermissions
							.Select(code => new System.Security.Claims.Claim("Permission", code)));
					}
				}

				// 3d. Return the new filtered principal
				return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(impersonatedClaims, "CustomAuth")));
			}

			// --- NEW CACHE POPULATION LOGIC ---

			// 1. Get the EmployeeId claim from the *working* principal
			var employeeIdClaim = fullPrincipal.Claims.FirstOrDefault(c => c.Type == "EmployeeId");

			if (employeeIdClaim != null && Guid.TryParse(employeeIdClaim.Value, out Guid employeeId))
			{
				// 2. Cache the EmployeeId immediately for the duration of the circuit
				_cacheService.SetEmployeeId(employeeId);
			}
			else
			{
				// 3. FALLBACK: If the claim is missing, we must perform the lookup here
				var fallbackUserIdClaim = fullPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

				if (fallbackUserIdClaim != null && Guid.TryParse(fallbackUserIdClaim.Value, out Guid fallbackUserId))
				{
					// WARNING: This requires injecting HRDbContext into the AuthenticationStateProvider, 
					// which is often unavoidable in this situation.

					// Assume you have HRDbContext injected:
					using var scope = _serviceScopeFactory.CreateScope(); // Use scope factory for safe DB access
					var dbContext = scope.ServiceProvider.GetRequiredService<HRDbContext>();

					var employee = await dbContext.Employees
						.AsNoTracking()
						.FirstOrDefaultAsync(e => e.UserId == fallbackUserId);

					if (employee != null)
					{
						_cacheService.SetEmployeeId(employee.Id);
						// Optional: You could now notify state change to update the principal, but we rely on the cache.
					}
				}
			}
			// --- END CACHE POPULATION LOGIC ---
			// 4. Return the full principal with up-to-date permissions
			return new AuthenticationState(fullPrincipal);
		}


		return new AuthenticationState(_anonymous);
	}

	// Inside CustomAuthenticationStateProvider.cs

	// Helper method to fetch and merge claims using IServiceProvider scope
	private async Task<IEnumerable<System.Security.Claims.Claim>> GetFullClaimsSet(Guid userId, System.Security.Claims.ClaimsIdentity baseIdentity)
	{
		// CRITICAL: I created a scope because HRDbContext is scoped, and we are not in the standard HTTP request scope.
		using (var scope = _serviceProvider.CreateScope())
		{
			// Resolve the required dependencies
			var dbContext = scope.ServiceProvider.GetRequiredService<HR.Data.HRDbContext>();

			// 1. Get the User's RoleId (Needed for RBAC)
			var user = await dbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);
			if (user == null) return baseIdentity.Claims.Where(c => c.Type != "Permission");

			Guid roleId = user.RoleId;

			// 2. Fetch permissions based on the user's RoleId (RBAC Baseline)
			var rolePermissions = await dbContext.RolePermissions
				.Where(rp => rp.RoleId == roleId)
				.Select(rp => rp.PermissionCode)
				.ToListAsync();

			// 3. Fetch individual UserPermissions (Individual Overrides)
			var userPermissions = await dbContext.UserPermissions
				.Where(up => up.UserId == userId)
				.Select(up => up.PermissionCode)
				.ToListAsync();

			// 4. Combine and deduplicate all permission codes
			var allPermissionCodes = rolePermissions
				.Concat(userPermissions)
				.Distinct();

			// 5. Convert codes to claims
			var customPermissionClaims = allPermissionCodes
				.Select(code => new System.Security.Claims.Claim("Permission", code))
				.ToList();

			// 6. Combine static cookie claims with fresh dynamic claims
			var allClaims = baseIdentity.Claims
				.Where(c => c.Type != "Permission")
				.Concat(customPermissionClaims)
				.ToList();

			return allClaims;
		}
	}


	public void NotifyStateChange()
	{
		// Force the provider to re-evaluate the authentication state immediately.
		NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
	}
}