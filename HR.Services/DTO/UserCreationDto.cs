using System.ComponentModel.DataAnnotations;

namespace HR.Services.DTO;

public class UserCreationDto
{
	[Required(ErrorMessage = "Username is required.")]
	[StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters.")]
	public string Username { get; set; } = string.Empty;

	[Required(ErrorMessage = "Email is required.")]
	[EmailAddress(ErrorMessage = "Invalid email format.")]
	[StringLength(150)]
	public string Email { get; set; } = string.Empty;

	[Required(ErrorMessage = "Password is required.")]
	[MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
	public string Password { get; set; } = string.Empty;

	[Required(ErrorMessage = "Role is required.")]
	public string RoleName { get; set; } = HR.Services.Constants.AppRoles.Employee; // Default to Employee
}