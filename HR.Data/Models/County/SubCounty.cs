using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Data.Models.County
{
	public class SubCounty
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = default!;

		// The Link to County
		public Guid CountyId { get; set; }
		public County? County { get; set; }
	}
}
