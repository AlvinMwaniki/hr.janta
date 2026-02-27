using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Data.Models.Recruitment
{
	public class JobListing
	{
		public Guid Id { get; set; }
		public Guid JobRequisitionId { get; set; }
		public JobRequisition? JobRequisition { get; set; }

		public string? ExternalTitle { get; set; }
		public string? ExternalDescription { get; set; }
		public string? Location { get; set; }
		public string? RequiredSkills { get; set; }
		public int? RequiredExperienceYears { get; set; }
		public string? RequiredEducationLevel { get; set; }


		public bool IsActive { get; set; }
		public DateTime PublishedAt { get; set; }
		public DateTime? ClosingDate { get; set; }

		public ICollection<JobApplication> Applications { get; set; } = new List<JobApplication>();
	}
}
