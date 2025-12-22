// HR.Web.Admin.Services/IAuthService.cs
using HR.Data.Models.Auth;

namespace HR.Services.Services;
public interface IAuthService
{
	// For verifying a user's password during login
	Task<User?> AuthenticateUserAsync(string email, string password);

	// For handling the secure hashing of passwords
	string HashPassword(User user, string password);
	bool VerifyPassword(string hashedPassword, string providedPassword);

	// For managing the one-time password setup token
	Task<(bool Success, Guid UserId)> ValidateSetupTokenAsync(string token);
	Task<string> GenerateAndSaveSetupTokenAsync(Guid userId);
	Task SetNewPasswordAsync(Guid userId, string newPassword);

	Task InitiateEmployeeSetupAsync(Guid userId, string email, string firstName, string lastName);
}