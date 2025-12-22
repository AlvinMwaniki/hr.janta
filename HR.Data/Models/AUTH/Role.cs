using System;

namespace HR.Data.Models.Auth;

public class Role
{
	public Guid Id { get; set; }
	public string Name { get; set; } = default!;
}
