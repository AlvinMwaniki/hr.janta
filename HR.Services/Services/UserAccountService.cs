using HR.Data;
using HR.Data.Models.Auth;
using HR.Services.Interfaces;
using HR.Services.DTO;
using HR.Services.Constants;
using Microsoft.EntityFrameworkCore;

namespace HR.Services.Services;

public class UserAccountService : IUserAccountService
{
	private readonly HRDbContext _dbContext;
	private readonly IAuthService _authService; // Secure hashing dependency

	// ⭐ Constructor: Injecting both DbContext and AuthService
	public UserAccountService(HRDbContext dbContext, IAuthService authService)
	{
		_dbContext = dbContext;
		_authService = authService;
	}

	public async Task<Guid> CreateUserAsync(UserCreationDto dto)
	{
		if (await _dbContext.Users.AnyAsync(u => u.Email == dto.Email))
		{
			throw new InvalidOperationException("A user with this email already exists.");
		}

		var role = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Name == dto.RoleName);
		if (role == null)
		{
			throw new InvalidOperationException($"Role '{dto.RoleName}' not found.");
		}

		var newUser = new User
		{
			Id = Guid.NewGuid(),
			Username = dto.Username,
			Email = dto.Email,
			RoleId = role.Id,
			CreatedAt = DateTime.UtcNow
		};

		// ⭐ Use the secure IAuthService to hash the password ⭐
		newUser.PasswordHash = _authService.HashPassword(newUser, dto.Password);

		_dbContext.Users.Add(newUser);
		await _dbContext.SaveChangesAsync();

		return newUser.Id;
	}

	// Method from interface: Get user data for the Edit page
	public async Task<UserEditDto?> GetUserForEditAsync(Guid userId)
	{
		var user = await _dbContext.Users
			.Include(u => u.Role)
			.AsNoTracking()
			.FirstOrDefaultAsync(u => u.Id == userId);

		if (user == null) return null;

		return new UserEditDto
		{
			Id = user.Id,
			Username = user.Username,
			Email = user.Email,
			// We now know RoleName exists in UserEditDto
			RoleName = user.Role?.Name ?? AppRoles.Employee
		};
	}

	// Method from interface: Update general user info (email, username, role)
	public async Task UpdateUserGeneralInfoAsync(UserEditDto dto)
	{
		var user = await _dbContext.Users
			.Include(u => u.Role)
			.FirstOrDefaultAsync(u => u.Id == dto.Id);

		if (user == null)
		{
			// For admin page, it's better to throw an exception if the user is somehow missing
			throw new InvalidOperationException($"User with ID {dto.Id} could not be found for update.");
		}

		// ⭐ 1. CRITICAL CHECK: Look for email duplication before proceeding ⭐
		// Check if any OTHER user (whose Id is NOT the current user's Id) 
		// already has the new email address.
		if (user.Email != dto.Email) // Only run the check if the email address has actually changed
		{
			var duplicateUser = await _dbContext.Users
				.AsNoTracking()
				.FirstOrDefaultAsync(u => u.Email == dto.Email && u.Id != dto.Id);

			if (duplicateUser != null)
			{
				// Throw a specific exception with the user-friendly message
				throw new InvalidOperationException(
					$"The email address '{dto.Email}' is already in use by another account ({duplicateUser.Username}). " +
					"Please use a unique email address."
				);
			}
		}
		// ⭐ End of CRITICAL CHECK ⭐
		// Check for role change
		if (user.Role?.Name != dto.RoleName)
		{
			var role = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Name == dto.RoleName);
			if (role == null) throw new InvalidOperationException($"Role '{dto.RoleName}' not found.");
			user.RoleId = role.Id;
		}

		// Update general details
		user.Username = dto.Username;
		user.Email = dto.Email;

		await _dbContext.SaveChangesAsync();
	}


	public async Task ChangePasswordAsync(UserPasswordChangeDto dto)
	{
		var user = await _dbContext.Users.FindAsync(dto.UserId);
		if (user == null) return;

		// ⭐ Use the secure IAuthService to hash the new password ⭐
		// This is the correct, secure approach using your existing infrastructure.
		user.PasswordHash = _authService.HashPassword(user, dto.NewPassword);

		await _dbContext.SaveChangesAsync();
	}
}