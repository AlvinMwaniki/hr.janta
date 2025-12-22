using HR.Data;
using HR.Data.Models.Auth;
using HR.Services.Constants;
using HR.Services.DTO; // Assuming DTO is where UserManagementDto is located
using HR.Services.Interfaces;

using Microsoft.EntityFrameworkCore;

using System.Security.Claims; // Needed for future proofing/checking

namespace HR.Services.Services;

public class PermissionManagementService : IPermissionManagementService
{
	private readonly HRDbContext _dbContext;
	private readonly ImpersonationService _impersonationService; // Needed to force UI refresh on role change

	public PermissionManagementService(HRDbContext dbContext, ImpersonationService impersonationService)
	{
		_dbContext = dbContext;
		_impersonationService = impersonationService;
	}

	// -----------------------------------------------------------------
	// READ OPERATION
	// -----------------------------------------------------------------

	public async Task<List<UserManagementDto>> GetAllUsersForManagementAsync()
	{
		// 1. Fetch Users and their associated Roles
		var users = await _dbContext.Users
			.Include(u => u.Role)
			.Include(u => u.UserPermissions) // NOTE: You need to add this property to your User.cs
			.AsNoTracking() // Read-only query for performance
			.ToListAsync();

		// 2. Map to DTO (Simulating claims for IsAdmin and CanSubmitLeave based on RoleName)
		var userDtos = users.Select(u => new UserManagementDto
		{
			Id = u.Id,
			Username = u.Username,
			Email = u.Email,
			CreatedDate = u.CreatedAt,
			RoleName = u.Role.Name,

			// --- SIMULATED PERMISSION CHECKS (BASED ON ROLE ID) ---
			IsAdmin = u.Role.Name == AppRoles.Admin,
			// For now, assume all Admins and regular Employees can SubmitLeave
			CanSubmitLeave = u.Role.Name == AppRoles.Admin || u.Role.Name == AppRoles.Employee
		}).ToList();

		return userDtos;
	}

	// -----------------------------------------------------------------
	// WRITE OPERATIONS
	// -----------------------------------------------------------------

	public async Task ToggleAdminRoleAsync(Guid userId, bool grantAdmin)
	{
		var user = await _dbContext.Users.FindAsync(userId);
		if (user == null) return;

		// 1. Find the target RoleId
		var targetRole = await _dbContext.Roles
			.FirstOrDefaultAsync(r => r.Name == (grantAdmin ? AppRoles.Admin : AppRoles.Employee));

		// FIX: Wrap the ternary operator in parentheses
		if (targetRole == null)
		{
			throw new InvalidOperationException($"Required role '{(grantAdmin ? AppRoles.Admin : AppRoles.Employee)}' not found in database.");
		}

		// 2. Update the user's RoleId
		user.RoleId = targetRole.Id;
		user.Role = targetRole; // Update navigation property if entity tracking is active

		await _dbContext.SaveChangesAsync();

		// 3. Force Authentication State Refresh (Crucial for the Admin being toggled)
		_impersonationService.ToggleAuthenticationStateRefresh();
	}

	// NOTE: This implementation is TEMPORARY/SIMPLIFIED because you don't have a custom claims table.
	// In a final system, this would update a UserPermissions table.
	public async Task TogglePermissionAsync(Guid userId, string permission, bool grant)
	{
		var existingPermission = await _dbContext.UserPermissions
			.FirstOrDefaultAsync(up => up.UserId == userId && up.PermissionCode == permission);

		if (grant)
		{
			if (existingPermission == null)
			{
				// GRANT: Insert a new record
				var newPermission = new UserPermission
				{
					Id = Guid.NewGuid(),
					UserId = userId,
					PermissionCode = permission
				};
				_dbContext.UserPermissions.Add(newPermission);
				await _dbContext.SaveChangesAsync();

				// CRITICAL: Refresh the UI to load the new claim immediately
				_impersonationService.ToggleAuthenticationStateRefresh();
			}
		}
		else // revoke
		{
			if (existingPermission != null)
			{
				// REVOKE: Delete the existing record
				_dbContext.UserPermissions.Remove(existingPermission);
				await _dbContext.SaveChangesAsync();

				// CRITICAL: Refresh the UI to remove the claim immediately
				_impersonationService.ToggleAuthenticationStateRefresh();
			}
		}
	}

	public async Task DeleteUserAsync(Guid userId)
	{
		var user = await _dbContext.Users.FindAsync(userId);
		if (user == null) return;

		_dbContext.Users.Remove(user);
		await _dbContext.SaveChangesAsync();
	}
}