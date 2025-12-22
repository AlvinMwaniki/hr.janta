using HR.Data.Models.Employees;

using System;

namespace HR.Data.Models.Advances;

public class SalaryAdvance
{
	public Guid Id { get; set; }

	public Guid EmployeeId { get; set; }
	public Employee Employee { get; set; } = default!;

	public decimal Amount { get; set; }
	public DateTime RequestDate { get; set; } = DateTime.UtcNow;
	public string Reason { get; set; } = default!; //reason
	public string Status { get; set; } = "Pending";
}
