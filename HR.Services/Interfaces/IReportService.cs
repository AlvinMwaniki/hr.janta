using HR.Core.DTOs.Reports;
using HR.Core.Enums;

using System;
using System.Threading.Tasks;

namespace HR.Services.Interfaces
{
	public interface IReportService
	{
		// For the Charts (Gender, Age, etc.)
		Task<DashboardStatsDTO> GetLiveDashboardStatsAsync();

		// For the Filter/Buttons (Dapper SQL logic)
		Task<DynamicReportResult> GetGenericDataAsync(string reportType, DepartmentType? dept, DateTime? from, DateTime? to);
	}
}