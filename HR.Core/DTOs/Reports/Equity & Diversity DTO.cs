using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Core.DTOs.Reports;

public class EquityDiversityReportDto
{
	public string FullName { get; set; } = string.Empty;
	public string Gender { get; set; } = string.Empty;
	public string EthnicityName { get; set; } = string.Empty;
	public string DisabilityStatus { get; set; } = string.Empty; // e.g. "None" or "Physical"
	public int Age { get; set; }
	public string DepartmentName { get; set; } = string.Empty;
}
