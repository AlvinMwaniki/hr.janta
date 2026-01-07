// HR.Services/Services/ClaimService.cs 

using HR.Data.Models.Auth; //  Role model
using HR.Services.Constants;
using HR.Data;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace HR.Services.Services;

public class ClaimService
{
	private readonly HRDbContext _db;

	public ClaimService(HRDbContext db) 
	{
		_db = db;
	}

	public ClaimsPrincipal CreatePrincipal(User user, Role role)
	{
		// Note: Using GetAwaiter().GetResult() is necessary here because 
		// the framework sometimes expects this method to be synchronous.
		return CreatePrincipalAsync(user, role).GetAwaiter().GetResult();
	}

	// ⭐ PRIMARY LOGIC: Fetch Employee ID during claim creation
	public async Task<ClaimsPrincipal> CreatePrincipalAsync(User user, Role role)
	{
		// 1. Fetch the corresponding Employee ID from the database
		var employee = await _db.Employees
			.AsNoTracking()
			.FirstOrDefaultAsync(e => e.UserId == user.Id); // Employee has a UserId FK

		// If employee is found, use its ID; otherwise, use an empty Guid.
		Guid employeeId = employee?.Id ?? Guid.Empty;

		var claims = new List<Claim>
		{
            // Basic User Identity Claims
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
			new Claim(ClaimTypes.Name, user.Username), // Use an appropriate user name field
            new Claim(ClaimTypes.Email, user.Email),

			new Claim("EmployeeId", employeeId.ToString()),
            
            // ⭐ 1. Add the fundamental Role Claim ⭐
			new Claim(ClaimTypes.Role, role.Name)
		};


		/* ⭐ 2. Map Role to Default Permissions (The Authorization Logic) ⭐
		if (role.Name == AppRoles.Admin)

		{
			claims.Add(new Claim(ClaimTypes.GroupSid, AppPermissions.ManageUsers)); 
			claims.Add(new Claim("Permission", AppPermissions.ViewAdminDashboard));
			claims.Add(new Claim("Permission", AppPermissions.SubmitLeave));
			claims.Add(new Claim("Permission", AppPermissions.ViewEmployeeDashboard));

		}
		else if (role.Name == AppRoles.Employee)
		{
			claims.Add(new Claim("Permission", AppPermissions.SubmitLeave));
			claims.Add(new Claim("Permission", AppPermissions.ViewEmployeeDashboard));
		}*/



		// FUTURE: If I implement custom delegation, query the DB here for extra permissions and add them.

		var identity = new ClaimsIdentity(claims, "CustomAuth");
		return new ClaimsPrincipal(identity);


	}
}