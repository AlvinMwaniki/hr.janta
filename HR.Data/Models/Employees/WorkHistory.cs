using System;
using System.ComponentModel.DataAnnotations;

namespace HR.Data.Models.Employees;

public class WorkHistory
{
	public Guid Id { get; set; }
	public Guid EmployeeId { get; set; }
	public Employee Employee { get; set; } = default!;

	[Required(ErrorMessage = "Job Title is required.")]
	public string JobTitle { get; set; } = default!;

	[Required(ErrorMessage = "Company Name is required.")]
	public string CompanyName { get; set; } = default!;

	[Required(ErrorMessage = "Company City is required.")]
	public string CompanyCity { get; set; } = default!;

	[Required(ErrorMessage = "Company Country is required.")]
	public string CompanyCountry { get; set; } = default!;

	[Required(ErrorMessage = "Job Duties are required.")]
	public string JobDuties { get; set; } = default!;

	[Required(ErrorMessage = "End Date is required.")]
	public DateTime JobFromDate { get; set; }
	[Required(ErrorMessage = "End Date is required.")]
	public DateTime JobToDate { get; set; }

	public bool IsCurrentJob { get; set; }
}
