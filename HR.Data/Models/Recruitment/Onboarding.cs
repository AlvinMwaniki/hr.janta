using System;
using System.ComponentModel.DataAnnotations;

namespace HR.Data.Models.Recruitment
{
	public class Onboarding
	{
		public Guid Id { get; set; }

		[Required]
		public Guid JobApplicationId { get; set; }
		public JobApplication JobApplication { get; set; } = default!;

		public bool KRAProvided { get; set; }
		public bool NSSFProvided { get; set; }
		public bool NHIFProvided { get; set; }
		public bool BankDetailsProvided { get; set; }
		public bool BackgroundCheckPassed { get; set; }

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public DateTime? UpdatedAt { get; set; }
	}
}