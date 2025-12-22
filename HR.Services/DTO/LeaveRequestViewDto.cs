// HR.Services/DTOs/LeaveRequestViewDto.cs

using HR.Core.Enums;

public class LeaveRequestViewDto
{
	public Guid Id { get; set; }

	// Employee Info
	public string EmployeeName { get; set; } = string.Empty;
	public string EmployeeJobTitle { get; set; } = string.Empty;

	// Leave Details
	public LeaveType LeaveType { get; set; }
	public DateTime FromDate { get; set; }
	public DateTime ToDate { get; set; }
	public int DurationInDays { get; set; }
	public string Reason { get; set; } = string.Empty;

	// Status/Metadata
	public LeaveStatus Status { get; set; }
	public DateTime CreatedAt { get; set; }

	// Actions for Admin
	public Guid EmployeeId { get; set; } // Needed for quick link to employee profile
}