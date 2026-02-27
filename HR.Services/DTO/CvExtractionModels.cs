using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.Json.Serialization;

namespace HR.Services.DTO
{
	public class CvAnalysisResult
	{
		[JsonPropertyName("full_name")]
		public string FullName { get; set; } = string.Empty;

		[JsonPropertyName("email")]
		public string Email { get; set; } = string.Empty;

		[JsonPropertyName("PhoneNumber")]
		public string PhoneNumber { get; set; } = string.Empty;

		[JsonPropertyName("education")]
		public List<EducationDto> Education { get; set; } = new();

		[JsonPropertyName("experience")]
		public List<ExperienceDto> Experience { get; set; } = new();

		[JsonPropertyName("suitability_score")]
		public int SuitabilityScore { get; set; }

		[JsonPropertyName("ranking_reason")]
		public string Analysis { get; set; } = string.Empty;

		[JsonPropertyName("skills_found")]
		public List<string> SkillsFound { get; set; } = new();
		public int TotalYearsExperience { get; set; }
public string SeniorityLevel { get; set; } = "";
public bool IsRejected { get; set; }

	}

	public class EducationDto
	{
		public string Institution { get; set; } = string.Empty;
		public string Field { get; set; } = string.Empty;
		public string Level { get; set; } = string.Empty;
		public DateTime? StartDate { get; set; }
		public DateTime? EndDate { get; set; }
	}

	public class ExperienceDto
	{
		public string Company { get; set; } = string.Empty;
		public string JobTitle { get; set; } = string.Empty;
		public string Responsibilities { get; set; } = string.Empty;
		public DateTime? StartDate { get; set; }
		public DateTime? EndDate { get; set; }
	}
}
