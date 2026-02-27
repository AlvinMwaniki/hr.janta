using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Services.DTO.Recruitment;

public class RequisitionApprovalDto
{
	public int ApprovalLevel { get; set; }

	public string Status { get; set; } = string.Empty;

	public string? Comments { get; set; }

	public DateTime? ActionDate { get; set; }

	public Guid? ActionByUserId { get; set; }
	public string? ActionByName { get; set; }

}

