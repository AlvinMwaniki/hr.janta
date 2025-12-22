// HR.Services.DTO/LeaveChartDataDto.cs

public class LeaveChartDataDto
{
	// Total Leave Requests by Status
	public int LeavesApproved { get; set; }
	public int LeavesPending { get; set; }
	public int LeavesRejected { get; set; }

	// Helper property to check if there is any data
	public bool HasData => LeavesApproved > 0 || LeavesPending > 0 || LeavesRejected > 0;
}