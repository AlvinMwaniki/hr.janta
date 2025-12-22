using System;
using System.ComponentModel.DataAnnotations;

namespace HR.Data.Models.Employees;

public class NextOfKin
{
	public Guid Id { get; set; }
	public Guid EmployeeId { get; set; }
	public Employee Employee { get; set; } = default!;

	[Required(ErrorMessage = "Full Name is required.")]
	public string FullName { get; set; } = default!;
	[Required(ErrorMessage = "Relationship is required.")]
	public string Relationship { get; set; } = default!;
	[Required(ErrorMessage = "Phone is required.")]
	public string Phone { get; set; } = default!;
	[Required(ErrorMessage = "Address is required.")]
	public string Address { get; set; } = default!;
}
