using HR.Data.Models.Auth;
using HR.Data.Models.Employees;

using System;

namespace HR.Data.Models.Leaves;

public class LeaveRequest
{
	public Guid Id { get; set; }

	public Guid EmployeeId { get; set; }
	public Employee Employee { get; set; } = default!;

	public string LeaveType { get; set; } = default!;
	public DateTime FromDate { get; set; }
	public DateTime ToDate { get; set; }
	public string Reason { get; set; } = default!;
	public string Status { get; set; } = "Pending";

	public Guid? ApprovedByUserId { get; set; }
	public User? ApprovedBy { get; set; }
	public DateTime CreatedAt { get; set; } = DateTime.Now;
}
