// HR.Services/DTOs/LeaveSubmissionDto.cs

using System.ComponentModel.DataAnnotations;

using HR.Core.Enums;

public class LeaveSubmissionDto
{


	[Required(ErrorMessage = "Leave Type is required.")]
	public LeaveType Type { get; set; }

	[Required(ErrorMessage = "Start Date is required.")]
	[DataType(DataType.Date)]
	public DateTime? StartDate { get; set; }

	[Required(ErrorMessage = "End Date is required.")]
	[DataType(DataType.Date)]
	public DateTime? EndDate { get; set; }

	[Required(ErrorMessage = "Reason is required.")]
	[StringLength(500, ErrorMessage = "Reason cannot exceed 500 characters.")]
	public string Reason { get; set; } = string.Empty;

	// Computed property for duration validation
	public int DurationInDays => StartDate.HasValue && EndDate.HasValue
		? (int)Math.Ceiling((EndDate.Value - StartDate.Value).TotalDays) + 1
		: 0;
}