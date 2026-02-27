using HR.Data.Models.Employees;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Services.DTO
{
	public class EmploymentContractModel
	{
		public string EmployeeName { get; set; } = string.Empty;
		public string JobTitle { get; set; } = string.Empty;
		public string DepartmentName { get; set; } = string.Empty;
		public DateTime StartDate { get; set; }
		public string Email { get; set; } = string.Empty;
		public string Phone { get; set; } = string.Empty;
		public string Estate { get; set; } = string.Empty;
		public string SubCounty { get; set; } = string.Empty;
		public string County { get; set; } = string.Empty;

		public List<EducationHistory> Education { get; set; } = new();
		public List<WorkHistory> WorkHistory { get; set; } = new();
	}
}
