// HR.Services/Interfaces/IPermissionManagementService.cs

using HR.Services.DTO;

using System;
using System.Threading.Tasks;

namespace HR.Services.Interfaces
{
	public interface IPermissionManagementService
	{
		// Fetches all users and maps their roles/claims to the DTO
		Task<List<UserManagementDto>> GetAllUsersForManagementAsync();

		// Toggles the primary Admin role
		Task ToggleAdminRoleAsync(Guid userId, bool grantAdmin);

		// Toggles a granular permission (like SubmitLeave)
		Task TogglePermissionAsync(Guid userId, string permission, bool grant);

		// Deletes a user account
		Task DeleteUserAsync(Guid userId);
	}
}