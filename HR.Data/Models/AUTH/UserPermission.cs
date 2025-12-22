// HR.Data.Models.Auth/UserPermission.cs

using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Data.Models.Auth;

public class UserPermission
{
	public Guid Id { get; set; }

	public Guid UserId { get; set; } // FK to the User table
	public string PermissionCode { get; set; } = default!; // Stores "Permissions.Leave.Submit"

	// Navigation Property
	public User User { get; set; } = default!;
}