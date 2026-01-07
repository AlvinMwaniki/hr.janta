using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Data.Models.Country
{
	public class Country
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = default!;
		public string IsoCode { get; set; } = default!; // e.g., "KE", "US"
		public string DialCode { get; set; } = default!; // e.g., "+254", "+1"
		public string FlagUrl => $"https://flagcdn.com/w40/{IsoCode.ToLower()}.png";
	}
}
