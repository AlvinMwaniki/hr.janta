using HR.Data.Models.Employees;

using System;
using System.Collections.Generic;

namespace HR.Data.Models.Departments;

public class Department
{
	public Guid Id { get; set; }
	public string Name { get; set; } = default!;
	public string? Description { get; set; }

	public List<Employee> Employees { get; set; } = new();
}
