using HR.Core.Enums;

using System;
using System.ComponentModel.DataAnnotations;

namespace HR.Services.DTO.Recruitment
{
	public class RequisitionCreateDto
	{
		[Required]
		public string JobTitle { get; set; } = string.Empty;

		[Required]
		public Guid DepartmentId { get; set; }

		[Required]
		public ContractType ContractType { get; set; }

		[Required]
		public decimal SalaryMin { get; set; }

		[Required]
		public decimal SalaryMax { get; set; }

		[Required]
		public string Description { get; set; } = string.Empty;

		[Required]
		public string? RequiredSkills { get; set; }
		public int? RequiredExperienceYears { get; set; }
		public string? RequiredEducationLevel { get; set; }


	}
}
