using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HR.Core.DTOs.Reports;

namespace HR.Data.Reporting
{
	public interface IReportRepository
	{
		Task<IEnumerable<ContractExpiryReportDto>> GetContractExpiryReportAsync();
		Task<IEnumerable<BankStatutoryReportDto>> GetBankStatutoryReportAsync(string? deptId = null);
		Task<IEnumerable<EquityDiversityReportDto>> GetEquityDiversityReportAsync();
		Task<DashboardStatsDTO> GetDashboardStatsAsync(); 
		Task<DynamicReportResult> GetDynamicReportAsync(string reportType, string? deptId, DateTime? from, DateTime? to);
		Task<byte[]> GenerateExcelReportAsync(List<IDictionary<string, object>> data, string reportName);
	}
}
