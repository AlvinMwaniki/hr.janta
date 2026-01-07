using HR.Services.Interfaces;
using HR.Services.DTO;
using HR.Data; 
using Microsoft.EntityFrameworkCore; 
using Microsoft.AspNetCore.Components.Authorization;

using System.Security.Claims;

namespace HR.Services.Services
{
	

	public class CurrentUserService : ICurrentUserService

	{
		// Ensure this constant matches the claim type used in your ClaimService.cs
		private readonly AuthenticationStateProvider _authenticationStateProvider;
		private readonly HRDbContext _db;

		// CRITICAL: It injects the Blazor authentication system's provider
		public CurrentUserService(AuthenticationStateProvider authenticationStateProvider, HRDbContext db)
		{
			_authenticationStateProvider = authenticationStateProvider;
			_db = db;
		}

		public async Task<Guid> GetCurrentUserIdAsync()
		{
			// 1. Get the current user's security context
			var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
			var principal = authState.User;

			var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);

			if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out Guid userId))
			{
				return userId;
			}
			return Guid.Empty;
		}

		// ⭐ IMPLEMENTATION 2: Get Display Data using the User ID
		public async Task<CurrentEmployeeDetailsDto?> GetCurrentEmployeeDetailsAsync()
		{
			var userId = await GetCurrentUserIdAsync();
			if (userId == Guid.Empty)
			{
				return null;
			}

			// Query the database using the User ID
			var employee = await _db.Employees
				.Include(e => e.Department) // Ensure Department is included for the name
				.AsNoTracking()
				.Where(e => e.UserId == userId)
				.Select(e => new CurrentEmployeeDetailsDto
				{
					FullName = e.FirstName + " " + e.LastName,

					JobTitle = e.JobTitle ?? "Not Assigned",

					DepartmentName = e.Department.Name
				})
				.FirstOrDefaultAsync();

			return employee;
		}

	}
}