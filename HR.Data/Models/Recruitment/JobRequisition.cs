using HR.Core.Enums;
using HR.Data.Models.Departments;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HR.Data.Models.Recruitment
{
	public class JobRequisition
	{
		public Guid Id { get; set; }

		[Required]
		public string RequisitionNumber { get; set; } = default!;

		[Required(ErrorMessage = "Job Title is required")]
		public string JobTitle { get; set; } = default!;

		[Required(ErrorMessage = "Department is required")]
		public Guid DepartmentId { get; set; }
		public Department? Department { get; set; } = default!;

		// ✅ Use existing ContractType
		[Required(ErrorMessage = "Employment Type is required")]
		public ContractType ContractType { get; set; }

		[Required(ErrorMessage = "Minimum Salary is required")]
		public decimal SalaryMin { get; set; }

		[Required(ErrorMessage = "Maximum Salary is required")]
		public decimal SalaryMax { get; set; }

		public bool BudgetApproved { get; set; } = false;

		[Required(ErrorMessage = "Description is required")]
		public string Description { get; set; } = default!;

		[Required]
		public Guid RequestedByUserId { get; set; }
		[Required]
		public string? RequiredSkills { get; set; }              // e.g. "C#, .NET, SQL"
		public int? RequiredExperienceYears { get; set; }        // e.g. 3
		public string? RequiredEducationLevel { get; set; }      // e.g. "Degree", "Diploma"


		public RequisitionStatus Status { get; set; } = RequisitionStatus.Draft;

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public DateTime? ApprovedAt { get; set; }

		// Navigation
		public virtual ICollection<RequisitionApproval> Approvals { get; set; } = new List<RequisitionApproval>();
	}
}
