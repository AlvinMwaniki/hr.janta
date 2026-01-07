using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Data.Models.County
{
	public class County
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = default!;
		public string Code { get; set; } = default!; // e.g., "047" for Nairobi

		// Navigation property to SubCounties
		// NEW: This tells EF that one County has MANY SubCounties
		public ICollection<SubCounty> SubCounties { get; set; } = new List<SubCounty>();
	}
}
