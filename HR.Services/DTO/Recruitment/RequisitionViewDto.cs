using HR.Core.Enums;

using System;

namespace HR.Services.DTO.Recruitment
{
	public class RequisitionViewDto
	{
		public Guid Id { get; set; }
		public string RequisitionNumber { get; set; } = default!;
		public string JobTitle { get; set; } = default!;
		public Guid DepartmentId { get; set; }
		public string DepartmentName { get; set; } = string.Empty; 
		public ContractType ContractType { get; set; }
		public decimal SalaryMin { get; set; }
		public decimal SalaryMax { get; set; }
		public RequisitionStatus Status { get; set; }
		public DateTime CreatedAt { get; set; }
		public string Description { get; set; } = string.Empty;
		public string? RequiredSkills { get; set; }
		public int? RequiredExperienceYears { get; set; }
		public string? RequiredEducationLevel { get; set; }


		public List<RequisitionApprovalDto> Approvals { get; set; } = new();

	}
}
