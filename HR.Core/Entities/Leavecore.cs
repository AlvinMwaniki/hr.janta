using HR.Core.Enums;

namespace HR.Core.Entities
{
	public class Leavecore
	{
		public Guid Id { get; set; }
		public Guid EmployeeId { get; set; }

		public LeaveType LeaveType { get; set; }
		public LeaveStatus Status { get; set; } = LeaveStatus.Pending;

		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }

		public Employeecore? Employee { get; set; }
		public string? Reason { get; set; }

		public DateTime CreatedAt { get; set; } =
	TimeZoneInfo.ConvertTimeFromUtc(
		DateTime.UtcNow,
		TimeZoneInfo.FindSystemTimeZoneById("E. Africa Standard Time")
	);

	}
}
