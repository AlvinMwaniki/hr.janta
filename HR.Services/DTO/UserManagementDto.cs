// HR.Services/Models/UserManagementDto.cs

using System;

namespace HR.Services.DTO;

public class UserManagementDto
{
	public Guid Id { get; set; }
	public string Username { get; set; } = default!;
	public string Email { get; set; } = default!;

	// Display Fields
	public string RoleName { get; set; } = default!; // Human-readable role
	public DateTime CreatedDate { get; set; }

	// Action Flags (based on User's current claims/roles)
	public bool IsAdmin { get; set; }
	public bool CanSubmitLeave { get; set; }
}