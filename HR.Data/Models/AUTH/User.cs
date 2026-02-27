using System;
using System.Data;
using System.Collections.Generic;

namespace HR.Data.Models.Auth;

public class User
{
	public Guid Id { get; set; }
	public string Username { get; set; } = default!;
	public string Email { get; set; } = default!;
	public string? PasswordHash { get; set; }
	public Guid RoleId { get; set; }
	public Role Role { get; set; } = default!;
	public bool IsActive { get; set; } = true;
	// ⭐ NEW: Navigation property for custom permissions ⭐
	public ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();

	public string? SetupToken { get; set; }
	public DateTime? TokenExpires { get; set; }

	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
