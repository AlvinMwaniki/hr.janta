using System;
using System.ComponentModel.DataAnnotations;

namespace HR.Data.Models.Employees;

public class EducationHistory
{
	public Guid Id { get; set; }
	public Guid EmployeeId { get; set; }
	public Employee Employee { get; set; } = default!;

	[Required(ErrorMessage = "School Name is required.")]
	public string SchoolName { get; set; } = default!;

	[Required(ErrorMessage = "Country is required.")]
	public string Country { get; set; } = default!;
	[Required(ErrorMessage = "Field of Study is required.")]
	public string Field { get; set; } = default!;
	[Required(ErrorMessage = "Level is required.")]
	public string Level { get; set; } = default!;
	[Required(ErrorMessage = "Start Date is required.")]
	public DateTime FromDate { get; set; }
	[Required(ErrorMessage = "End Date is required.")]
	public DateTime ToDate { get; set; }
}
