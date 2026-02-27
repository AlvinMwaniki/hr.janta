using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Core.DTOs.Reports;

public class BankStatutoryReportDto
{
	public string EmployeeCode { get; set; } = string.Empty;
	public string FullName { get; set; } = string.Empty;
	public string DepartmentName { get; set; } = string.Empty;

	// From PaymentData
	public string KRA_PIN { get; set; } = "N/A";
	public string NSSF_Number { get; set; } = "N/A";
	public string NHIF_Number { get; set; } = "N/A";

	// From BankDetail
	public string BankName { get; set; } = "N/A";
	public string AccountNumber { get; set; } = "N/A";
	public string Branch { get; set; } = "N/A";
}
