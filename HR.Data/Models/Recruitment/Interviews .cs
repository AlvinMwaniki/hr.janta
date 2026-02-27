using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Data.Models.Recruitment
{
	public enum InterviewOutcome
	{
		Pending,
		Passed,
		Failed
	}

	public class Interview
	{
		public Guid Id { get; set; }

		[Required]
		public Guid JobApplicationId { get; set; }
		public JobApplication JobApplication { get; set; } = default!;

		public DateTime InterviewDate { get; set; }
		public string? InterviewTime { get; set; } // e.g. "10:30 AM"
		public string? Notes { get; set; }

		public InterviewOutcome Outcome { get; set; } = InterviewOutcome.Pending;

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public DateTime? UpdatedAt { get; set; }
	}


}
