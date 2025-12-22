// HR.Data.Models.Auth/RolePermission.cs

using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Data.Models.Auth;

public class RolePermission
{
	public Guid Id { get; set; }

	public Guid RoleId { get; set; } // FK to the Role table (The central piece of RBAC)
	public string PermissionCode { get; set; } = default!; // Stores "Permissions.Leave.Submit"

	// Navigation Properties
	public Role Role { get; set; } = default!;
}