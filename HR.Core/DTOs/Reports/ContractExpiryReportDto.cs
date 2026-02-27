using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Core.DTOs.Reports
{
	public class ContractExpiryReportDto
	{
		// Change from int to string to match MySQL char(36)
		public string EmployeeId { get; set; } = string.Empty;

		public string EmployeeName { get; set; } = string.Empty;
		public string DepartmentName { get; set; } = string.Empty;
		public DateTime ContractEndDate { get; set; }

		public int DaysRemaining => (ContractEndDate.Date - DateTime.Today).Days;

		public string Status => DaysRemaining switch
		{
			< 0 => "Expired",
			<= 30 => "Critical",
			<= 90 => "Warning",
			_ => "Healthy"
		};
	}
}
