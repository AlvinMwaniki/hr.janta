namespace HR.Services.DTO
{
	public class DashboardTotals
	{
		public int Employees { get; set; }
		public int Departments { get; set; }
		public int Leaves { get; set; }
		public int Advances { get; set; }

		public int OnLeave { get; set; }
		public int NotOnLeave { get; set; }

		public int LeaveBalance { get; set; }
		public int LeavesTaken { get; set; }
		public DateTime? LastPayslipDate { get; set; }

		// ⭐ NEW EMPLOYEE PROFILE INFO ⭐
		public string? JobTitle { get; set; }
		public string? Department { get; set; }
		public int PendingTasks { get; set; } // New metric for tasks

		// ⭐ NEW PROPERTIES FOR LEAVE CHART (Total Leave Requests by Status) ⭐
		public int LeavesApproved { get; set; }
		public int LeavesPending { get; set; }
		public int LeavesRejected { get; set; }
		public LeaveChartDataDto LeaveChartData { get; set; } = new LeaveChartDataDto();
		public AdvanceChartDataDto AdvanceChartData { get; set; } = new AdvanceChartDataDto();

		// ⭐ NEW EMPLOYEE PROFILE INFO ⭐
		//public string? JobTitle { get; set; }
		//public string? Department { get; set; }
		//public int PendingTasks { get; set; } // New metric for tasks
	}
}
