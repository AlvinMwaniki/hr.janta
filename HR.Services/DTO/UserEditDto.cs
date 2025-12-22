using System;
using System.ComponentModel.DataAnnotations;

namespace HR.Services.DTO;

public class UserEditDto
{
	public Guid Id { get; set; }

	[Required(ErrorMessage = "Email is required.")]
	[EmailAddress(ErrorMessage = "Invalid email format.")]
	public string Email { get; set; } = default!;

	[Required]
	public string Username { get; set; } = default!;

	// ⭐ FIX: ADD THIS MISSING PROPERTY ⭐
	[Required(ErrorMessage = "Role is required.")]
	public string RoleName { get; set; } = default!;



	// Optional: Only include if the employee data is part of the UserEdit scope
	public Guid? LinkedEmployeeId { get; set; }
}

public class UserPasswordChangeDto
{
	public Guid UserId { get; set; }

	[Required, MinLength(8)]
	public string NewPassword { get; set; } = default!;

	[Compare(nameof(NewPassword))]
	public string ConfirmPassword { get; set; } = default!;
}