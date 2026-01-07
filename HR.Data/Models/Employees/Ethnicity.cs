using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Data.Models.Employees
{
	public class Ethnicity
	{
		public Guid Id { get; set; }
		[Required]
		public string Name { get; set; } = default!;
	}
}