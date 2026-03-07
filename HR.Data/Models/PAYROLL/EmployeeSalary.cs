using HR.Data.Models.Employees;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Data.Models.PAYROLL
{
	public class EmployeeSalary
	{
		public Guid Id { get; set; }
		public Guid EmployeeId { get; set; }
		public Employee? Employee { get; set; }
		public string? JobTitle { get; set; }
		public Guid? DepartmentId { get; set; }
		public decimal BasicSalary { get; set; }
		public DateTime EffectiveDate { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	}
}
