using HR.Core.Enums;
using HR.Data.Models.Auth;

using System;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Data.Models.Recruitment
{
	public class RequisitionApproval
	{
		public Guid Id { get; set; }

		[Required]
		public Guid JobRequisitionId { get; set; }
		public JobRequisition JobRequisition { get; set; } = default!;

		public Guid? ActionByUserId { get; set; }
		public virtual User? ActionByUser { get; set; } = default!;
		[Required]
		public ApprovalStatus Status { get; set; }

		public string? Comments { get; set; }

		public int ApprovalLevel { get; set; } = 1;

		public DateTime ActionDate { get; set; } = DateTime.UtcNow;
	}
}
