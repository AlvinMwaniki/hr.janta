// HR.Web.Admin.Services/AuthService.cs
using HR.Data.Models.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components; // <-- Need this for NavigationManager
using HR.Data;

namespace HR.Services.Services;
public class AuthService : IAuthService
{
	private readonly IPasswordHasher<User> _passwordHasher;
	private readonly HRDbContext _db;
	private readonly IEmailService _emailService; // <-- NEW
	private readonly NavigationManager _navigationManager; // <-- NEW

	public AuthService(HRDbContext db, IEmailService emailService, NavigationManager navigationManager)
	{
		_db = db;
		_passwordHasher = new PasswordHasher<User>();
		_emailService = emailService;
		_navigationManager = navigationManager;
	}

	// Hashing
	public string HashPassword(User user, string password)
	{
		return _passwordHasher.HashPassword(user, password);
	}

	public bool VerifyPassword(string hashedPassword, string providedPassword)
	{
		// For verification, we create a dummy user instance (as the hasher API requires it)
		var user = new User();
		var result = _passwordHasher.VerifyHashedPassword(user, hashedPassword, providedPassword);
		return result == PasswordVerificationResult.Success;
	}

	// Authentication (Implementation needed later)
	public async Task<User?> AuthenticateUserAsync(string email, string password)
	{
		var user = await _db.Users.Include(u => u.Role)
							 .FirstOrDefaultAsync(u => u.Email == email);

		if (user == null || user.PasswordHash == null)
		{
			return null;
		}

		// Use the VerifyPassword method defined above
		if (VerifyPassword(user.PasswordHash, password))
		{
			return user;
		}
		return null;
	}

	// HR.Web.Admin.Services/AuthService.cs (Implementation Section)

	// ... existing code, including AuthenticateUserAsync ...

	// Token Management

	/// Generates a unique token, saves it to the user's record, and sets an expiration time.
	public async Task<string> GenerateAndSaveSetupTokenAsync(Guid userId)
	{
		var user = await _db.Users.FindAsync(userId);
		if (user == null)
		{
			return string.Empty;
		}

		var token = Guid.NewGuid().ToString("N");

		user.SetupToken = token;
		user.TokenExpires = DateTime.UtcNow.AddHours(48);

		await _db.SaveChangesAsync();
		return token;
	}

	/// Validates the provided token against the database and clears it upon success.
	public async Task<(bool Success, Guid UserId)> ValidateSetupTokenAsync(string token)
	{
		if (string.IsNullOrEmpty(token))
		{
			return (false, Guid.Empty);
		}

		var user = await _db.Users
							.FirstOrDefaultAsync(u =>
								u.SetupToken == token &&
								u.TokenExpires.HasValue &&
								u.TokenExpires.Value > DateTime.UtcNow);

		if (user == null)
		{
			return (false, Guid.Empty);
		}

		// Token is valid. Clear it immediately to prevent reuse.
		user.SetupToken = null;
		user.TokenExpires = null;
		await _db.SaveChangesAsync();

		return (true, user.Id);
	}

	/// Hashes the new password and updates the user's record.
	public async Task SetNewPasswordAsync(Guid userId, string newPassword)
	{
		var user = await _db.Users.FindAsync(userId);
		if (user == null)
		{
			throw new ArgumentException($"User with ID {userId} not found during password set.");
		}

		user.PasswordHash = HashPassword(user, newPassword);

		user.SetupToken = null;
		user.TokenExpires = null;

		await _db.SaveChangesAsync();
	}
	public async Task InitiateEmployeeSetupAsync(Guid userId, string email, string firstName, string lastName)
	{
		// 1. Generate and save the secure one-time token
		var token = await GenerateAndSaveSetupTokenAsync(userId);

		if (string.IsNullOrEmpty(token))
		{
			throw new InvalidOperationException($"Could not generate setup token for user {userId}.");
		}

		// 2. Construct the setup link (e.g., https://hr.prox.com/set-password?token=XYZ)
		// NavigationManager builds the correct absolute URL for the employee to click.
		var setupUri = _navigationManager.ToAbsoluteUri($"/set-password?token={token}");

		// 3. Send the email
		string recipientName = $"{firstName} {lastName}";
		await _emailService.SendSetupPasswordEmailAsync(email, recipientName, setupUri.ToString());
	}
}
