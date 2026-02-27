using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HR.Data.Models.Recruitment
{
	public enum ApplicationStatus
	{
		New,
		Reviewing,
		Shortlisted,
		Interviewed,
		Rejected,
		Hired
	}

	public class JobApplication
	{
		public Guid Id { get; set; }

		[Required]
		public Guid JobListingId { get; set; }
		public JobListing? JobListing { get; set; }

		// Personal Info
		[Required(ErrorMessage = "Full Name is required")]
		public string FullName { get; set; } = default!;

		[Required, EmailAddress]
		public string Email { get; set; } = default!;

		[Required]
		public string PhoneNumber { get; set; } = default!;

		[Required(ErrorMessage = "Country is required")]
		public Guid? CountryId { get; set; }
		public HR.Data.Models.Country.Country? Country { get; set; }

		public Guid? CountyId { get; set; }
		public HR.Data.Models.County.County? County { get; set; }

		public Guid? SubCountyId { get; set; }
		public HR.Data.Models.County.SubCounty? SubCounty { get; set; }

		public string? Estate { get; set; }
		public string? POBox { get; set; }


		public string? LinkedInProfile { get; set; }

		// Documents
		[Required]
		public string CVPath { get; set; } = default!;
		public string? CoverLetter { get; set; }

		// AI ATS Features (Smart Ranking)
		public int SuitabilityScore { get; set; } // 0-100
		public string? AIRankingReason { get; set; }
		public string? SkillsFound { get; set; }

		// Collections (Mapped to Applicant History)
		public virtual ICollection<ApplicantEducation> Education { get; set; } = new List<ApplicantEducation>();
		public virtual ICollection<ApplicantExperience> Experience { get; set; } = new List<ApplicantExperience>();

		public ApplicationStatus Status { get; set; } = ApplicationStatus.New;
		public DateTime AppliedAt { get; set; } = DateTime.UtcNow;
	}

	public class ApplicantEducation
	{
		public Guid Id { get; set; }
		public Guid JobApplicationId { get; set; }
		public virtual JobApplication JobApplication { get; set; } = default!;

		public string Institution { get; set; } = default!;
		public string Field { get; set; } = default!;
		public string Level { get; set; } = default!;
		public string? Country { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime? EndDate { get; set; }
	}

	public class ApplicantExperience
	{
		public Guid Id { get; set; }
		public Guid JobApplicationId { get; set; }
		public virtual JobApplication JobApplication { get; set; } = default!;

		public string Company { get; set; } = default!;
		public string JobTitle { get; set; } = default!;
		public string Responsibilities { get; set; } = default!;
		public string? City { get; set; }
		public string? Country { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime? EndDate { get; set; }
	}
}