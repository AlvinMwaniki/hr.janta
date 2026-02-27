using HR.Core.Enums;
using HR.Data.Models.Employees;

namespace HR.Data.Models.EmployestatusLog
{
	public class EmployeeStatusLog
	{
		public Guid Id { get; set; } = Guid.NewGuid();
		public Guid EmployeeId { get; set; }

		// The new state (Terminated, Suspended, etc.)
		public EmployeeStatus NewStatus { get; set; }

		public DateTime EffectiveDate { get; set; } = DateTime.Now;
		public DateTime? NoticeDate { get; set; }
		public DateTime? LastWorkDate { get; set; }
		// Details from your Exit_Staff logic
		public string? Reason { get; set; }
		public string? Notes { get; set; }
		public string AuthorizedBy { get; set; } = default!;
		// Audit fields
		public bool IsRehireable { get; set; }
		public bool FinalPayProcessed { get; set; }
		public decimal FinalSettlementAmount { get; set; }

		// Navigation
		public virtual Employee Employee { get; set; } = default!;
	}
}