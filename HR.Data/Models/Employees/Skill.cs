using System;

namespace HR.Data.Models.Employees;

public class Skill
{
	public Guid Id { get; set; }
	public Guid EmployeeId { get; set; }
	public Employee Employee { get; set; } = default!;

	public string Name { get; set; } = default!;
	public string Level { get; set; } = default!; // Beginner, Intermediate, Expert
}
