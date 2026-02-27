using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace HR.Core.DTOs.Reports

{
	public class DashboardStatsDTO
	{
		public double MaleCount { get; set; }
		public double FemaleCount { get; set; }
		public double GenZCount { get; set; }
		public double MillennialCount { get; set; }
		public double OlderCount { get; set; }
		// Page 2: Trend Analysis
		public double[] MonthlyLeavesTrend { get; set; } = new double[12];
		public double[] MonthlyAdvancesTrend { get; set; } = new double[12];

		public double CurrentMonthLeaves { get; set; }
		public double CurrentMonthAdvances { get; set; }

		// Page 2: Staff Composition
		public int PermanentCount { get; set; }
		public int ContractCount { get; set; }
		public int InternCount { get; set; }
		public int TotalCount { get; set; }
		public List<EthnicityStat> EthnicityDistribution { get; set; } = new();
		public List<EthnicityStat> Ethnicities { get; set; } = new();

		public int MonthlyEntries { get; set; }
		public int YearlyExits { get; set; }
		public int TotalDepartments { get; set; }
		public List<DepartmentDistributionDto> DeptDistribution { get; set; } = new();
	}
	public class DepartmentDistributionDto
	{
		public string Name { get; set; } = "";
		public int EmployeeCount { get; set; }
		public double Percentage { get; set; } 
	}
	public class EthnicityStat
	{
		public string Name { get; set; } = "";
		public double Percentage { get; set; }
	}
	public class DynamicReportResult
	{
		// This holds the column names (e.g., "Staff Name", "Days Taken")
		public List<string> Columns { get; set; } = new();

		// This holds the actual data rows
		public List<IDictionary<string, object>> Data { get; set; } = new();
	}
}
