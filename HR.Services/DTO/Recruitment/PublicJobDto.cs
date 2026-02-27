using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Services.DTO.Recruitment
{
	public class PublicJobDto
	{
		public Guid ListingId { get; set; }
		public string Title { get; set; } = string.Empty;
		public string Location { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public string ContractType { get; set; } = string.Empty;
		public DateTime PublishedAt { get; set; }
	}
}
