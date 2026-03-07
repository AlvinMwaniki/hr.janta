using Dapper;

using HR.Core.DTOs.Reports;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

using MySqlConnector;
using ClosedXML.Excel; 
using System.Data;
using System.Drawing;

namespace HR.Data.Reporting;

public class ReportRepository : IReportRepository
{
	private readonly string _connectionString;

	public ReportRepository(IConfiguration configuration)
	{
		// Match the key from your appsettings.json
		_connectionString = configuration.GetConnectionString("HRDbConnection")
							?? throw new InvalidOperationException("Connection string 'HRDbConnection' not found.");
	}

	public async Task<IEnumerable<ContractExpiryReportDto>> GetContractExpiryReportAsync()
	{
		// Use MySqlConnection for MySQL
		using IDbConnection db = new MySqlConnection(_connectionString);

		// MySQL uses CONCAT() instead of the '+' operator
		const string sql = @"
    SELECT 
        CAST(e.Id AS CHAR) as EmployeeId, 
        CONCAT(e.FirstName, ' ', e.LastName) as EmployeeName, 
        d.Name as DepartmentName, 
        e.ContractEndDate
    FROM Employees e
    LEFT JOIN Departments d ON e.DepartmentId = d.Id
    WHERE e.ContractEndDate IS NOT NULL
    ORDER BY e.ContractEndDate ASC";

		return await db.QueryAsync<ContractExpiryReportDto>(sql);
	}
	public async Task<IEnumerable<BankStatutoryReportDto>> GetBankStatutoryReportAsync(string? deptId = null)
	{
		using IDbConnection db = new MySqlConnection(_connectionString);
		string sql = @"
        SELECT e.EmployeeCode, CONCAT(e.FirstName, ' ', e.LastName) as FullName,d.Name as DepartmentName,
               p.KRA_PIN, p.NSSF_Number, p.NHIF_Number,
               b.BankName, b.AccountNumber, b.Branch
        FROM Employees e
        INNER JOIN departments d ON e.DepartmentId = d.Id
        LEFT JOIN PaymentData p ON e.Id = p.EmployeeId
        LEFT JOIN BankDetails b ON p.BankDetailId = b.Id
        WHERE 1=1";

		if (!string.IsNullOrEmpty(deptId) && deptId != "All")
		{
			sql += " AND e.DepartmentId = @DeptId";
		}

		return await db.QueryAsync<BankStatutoryReportDto>(sql, new { DeptId = deptId });
	}
	

	public async Task<IEnumerable<EquityDiversityReportDto>> GetEquityDiversityReportAsync()
	{
		using IDbConnection db = new MySqlConnection(_connectionString);

		string sql = @"
        SELECT 

            CAST(e.Id AS CHAR) as EmployeeId,
            CONCAT(e.FirstName, ' ', e.LastName) as FullName,
            e.Gender,
            eth.Name as EthnicityName,
            e.Disability as DisabilityStatus,
            TIMESTAMPDIFF(YEAR, e.DOB, CURDATE()) as Age,
            d.Name as DepartmentName
        FROM Employees e
        LEFT JOIN Ethnicities eth ON e.EthnicityId = eth.Id
        INNER JOIN Departments d ON e.DepartmentId = d.Id";

		return await db.QueryAsync<EquityDiversityReportDto>(sql);
	}
	public async Task<DashboardStatsDTO> GetDashboardStatsAsync()
	{
		using IDbConnection db = new MySqlConnection(_connectionString);

		// SQL ORDER: 
		// 1. Gender 
		// 2. Age 
		// 3. Ethnicity 
		// 4. Monthly Entries 
		// 5. Yearly Exits 
		// 6. Dept Distribution 
		// 7. Contract Type
		const string sql = @"
        /* 1. Gender Counts */
        SELECT Gender, COUNT(*) as Count FROM Employees GROUP BY Gender;

        /* 2. Age Demographics */
        SELECT 
            SUM(CASE WHEN TIMESTAMPDIFF(YEAR, DOB, CURDATE()) < 25 THEN 1 ELSE 0 END) as GenZ,
            SUM(CASE WHEN TIMESTAMPDIFF(YEAR, DOB, CURDATE()) BETWEEN 25 AND 40 THEN 1 ELSE 0 END) as Millennial,
            SUM(CASE WHEN TIMESTAMPDIFF(YEAR, DOB, CURDATE()) > 40 THEN 1 ELSE 0 END) as Boomers
        FROM Employees;

        /* 3. Ethnicity Distribution */
        SELECT eth.Name, COUNT(e.Id) as Total 
        FROM Ethnicities eth
        LEFT JOIN Employees e ON eth.Id = e.EthnicityId
        GROUP BY eth.Name;

        /* 4. Monthly Entries */
        SELECT COUNT(*) FROM Employees WHERE MONTH(CreatedAt) = MONTH(CURDATE()) AND YEAR(CreatedAt) = YEAR(CURDATE());

        /* 5. Yearly Exits */
        SELECT COUNT(*) FROM EmployeeStatusLogs WHERE NewStatus >= 4 AND YEAR(EffectiveDate) = YEAR(CURDATE());

        /* 6. Dept Distribution */
        SELECT d.Name, COUNT(e.Id) as EmpCount 
        FROM Departments d 
        LEFT JOIN Employees e ON d.Id = e.DepartmentId 
        GROUP BY d.Name;

        /* 7. Contract Type Distribution */
        SELECT ContractType as CType, COUNT(*) as CCount FROM Employees GROUP BY ContractType;

        /* 8. Monthly Leaves Trend for Current Year */
         SELECT MONTH(FromDate) as Month, COUNT(*) as Count 
         FROM LeaveRequests 
         WHERE YEAR(FromDate) = YEAR(CURDATE())
         GROUP BY MONTH(FromDate);

         /* 9. Monthly Advances Trend for Current Year */
         SELECT MONTH(RequestDate) as Month, COUNT(*) as Count 
          FROM SalaryAdvances 
         WHERE YEAR(RequestDate) = YEAR(CURDATE())
         GROUP BY MONTH(RequestDate);

         /* 10. ⭐ TRUE TOTAL — counts ALL employees regardless of gender/missing data */
         SELECT COUNT(*) FROM Employees;";



		using var multi = await db.QueryMultipleAsync(sql);
		var stats = new DashboardStatsDTO();

		// 1. Gender (Matches SQL 1)
		var genderRows = (await multi.ReadAsync<(string Gender, int Count)>()).ToList();
		stats.MaleCount = genderRows.FirstOrDefault(x => x.Gender == "Male").Count;
		stats.FemaleCount = genderRows.FirstOrDefault(x => x.Gender == "Female").Count;
		var totalEmployees = genderRows.Sum(x => x.Count);      // 2. Age (Matches SQL 2)
		var ageRow = await multi.ReadFirstAsync<dynamic>();
		stats.GenZCount = (double)(ageRow.GenZ ?? 0);
		stats.MillennialCount = (double)(ageRow.Millennial ?? 0);
		stats.OlderCount = (double)(ageRow.Boomers ?? 0);

		// 3. Ethnicity (Matches SQL 3)
		var ethRows = await multi.ReadAsync<dynamic>();
		foreach (var row in ethRows)
		{
			double percentage = totalEmployees > 0 ? (Convert.ToDouble(row.Total) / totalEmployees) * 100 : 0;
			stats.Ethnicities.Add(new EthnicityStat { Name = row.Name, Percentage = Math.Round(percentage, 1) });
		}

		// 4. Monthly Entries (Matches SQL 4)
		stats.MonthlyEntries = await multi.ReadFirstAsync<int>();

		// 5. Yearly Exits (Matches SQL 5)
		stats.YearlyExits = await multi.ReadFirstAsync<int>();

		// 6. Dept Distribution (Matches SQL 6)
		var deptRows = (await multi.ReadAsync<dynamic>()).ToList(); 
		foreach (var d in deptRows)
		{
			stats.DeptDistribution.Add(new DepartmentDistributionDto
			{
				Name = d.Name,
				EmployeeCount = (int)d.EmpCount,
				Percentage = totalEmployees > 0 ? (Convert.ToDouble(d.EmpCount) / totalEmployees) * 100 : 0
			});
		}
		stats.TotalDepartments = stats.DeptDistribution.Count;

		// 7. Contract Type (Matches SQL 7) - NOW THIS WILL WORK
		// 7. Contract Type
		var contractRows = (await multi.ReadAsync<dynamic>()).ToList();

		stats.PermanentCount = 0;
		stats.ContractCount = 0;
		stats.InternCount = 0;

		foreach (var row in contractRows)
		{
			string dbType = Convert.ToString(row.CType) ?? "";
			int dbCount = Convert.ToInt32(row.CCount);

			if (dbType == "0") stats.PermanentCount = dbCount;  // 7 Permanent
			else if (dbType == "1") stats.ContractCount = dbCount;  // 16 Contract
			else if (dbType == "2") stats.InternCount = dbCount;  // 1 Intern
		}
		// ... after reading Contract Types ...

		// 8. Map Monthly Leaves
		var leaveTrendRows = (await multi.ReadAsync<(int Month, int Count)>()).ToList();
		foreach (var row in leaveTrendRows)
		{
			// Month is 1-based (Jan=1), Array is 0-based
			stats.MonthlyLeavesTrend[row.Month - 1] = row.Count;
		}

		// 9. Map Monthly Advances
		var advanceTrendRows = (await multi.ReadAsync<(int Month, int Count)>()).ToList();
		foreach (var row in advanceTrendRows)
		{
			stats.MonthlyAdvancesTrend[row.Month - 1] = row.Count;
		}
		// 10. ⭐ TRUE TOTAL — read last
		totalEmployees = await multi.ReadFirstAsync<int>(); // ⭐ no 'int' — just reassign
		stats.TotalCount = totalEmployees;

		// Set the "Current Month" single values for  cards
		int currentMonth = DateTime.Now.Month;
		stats.CurrentMonthLeaves = stats.MonthlyLeavesTrend[currentMonth - 1];
		stats.CurrentMonthAdvances = stats.MonthlyAdvancesTrend[currentMonth - 1];

		return stats;
	}

	public async Task<DynamicReportResult> GetDynamicReportAsync(string reportType, string? deptId, DateTime? from, DateTime? to)
	{
		using IDbConnection db = new MySqlConnection(_connectionString);

		// 1. ROUTE TO SPECIALIZED METHODS (FOR DATA-HEAVY DTOs)
		if (reportType == "BankReport")
			return MapToDynamicResult((await GetBankStatutoryReportAsync(deptId)).Cast<object>());

		if (reportType == "ContractExpiry")
			return MapToDynamicResult((await GetContractExpiryReportAsync()).Cast<object>());

		if (reportType == "EquityDiversity")
			return MapToDynamicResult((await GetEquityDiversityReportAsync()).Cast<object>());

		// 2. DYNAMIC QUERIES BASED ON YOUR SPECIFIC MODELS
		string sql = reportType switch
		{
			// Querying the EmployeeStatusLog table for Exits (Status >= 4)
			"ExitStaff" => @"SELECT e.EmployeeCode, CONCAT(e.FirstName, ' ', e.LastName) as EmployeeName, 
                         log.NewStatus as ExitType, log.EffectiveDate as ExitDate, log.Reason 
                         FROM EmployeeStatusLogs log 
                         JOIN Employees e ON log.EmployeeId = e.Id 
                         WHERE log.NewStatus >= 4",

			// Querying the SalaryAdvances table
			"AdvancesApplied" => @"SELECT e.EmployeeCode, CONCAT(e.FirstName, ' ', e.LastName) as EmployeeName, 
                                   a.Amount, a.RequestDate, a.Status, a.Reason
                                   FROM SalaryAdvances a 
                                   JOIN Employees e ON a.EmployeeId = e.Id 
                                   WHERE 1=1",

			"EmployeeList" => "SELECT e.EmployeeCode, e.FirstName, e.LastName, e.JobTitle, e.Email, e.Phone FROM Employees e WHERE 1=1",
			"LeavesApplied" => @"SELECT e.EmployeeCode, CONCAT(e.FirstName, ' ', e.LastName) as Employee, 
                              l.LeaveType, l.FromDate as StartDate, l.ToDate as EndDate, l.Status
                              FROM LeaveRequests l 
                              JOIN Employees e ON l.EmployeeId = e.Id 
                              WHERE 1=1",
			"RejectedLeaves" => @"SELECT e.EmployeeCode, CONCAT(e.FirstName, ' ', e.LastName) as Employee, 
                                  l.LeaveType, l.FromDate as StartDate, l.ToDate as EndDate, l.Status
                                  FROM LeaveRequests l 
                                  JOIN Employees e ON l.EmployeeId = e.Id 
                                  WHERE l.Status = 'Rejected'",

			"LeavesNotApplied" => @"SELECT e.EmployeeCode, 
                                   CONCAT(e.FirstName, ' ', e.LastName) as EmployeeName, 
                                   d.Name as Department,
                                   e.JobTitle,
                                   e.AnnualLeaveBalanceDays as RemainingDays
                            FROM Employees e
                            JOIN Departments d ON e.DepartmentId = d.Id
                            WHERE e.AnnualLeaveBalanceDays >= 21",


			"ApprovedLeaves" => @"SELECT e.EmployeeCode, CONCAT(e.FirstName, ' ', e.LastName) as Employee, 
                              l.LeaveType, l.FromDate as StartDate, l.ToDate as EndDate, l.Status
                              FROM LeaveRequests l 
                              JOIN Employees e ON l.EmployeeId = e.Id 
                              WHERE l.Status = 'Approved'
",


			_ => "SELECT 'Info' as Status, 'Report logic missing in Repository' as Message"
		};

		// 3. APPLY SHARED FILTERS (Date and Department)
		var parameters = new DynamicParameters();
		parameters.Add("DeptId", deptId);
		parameters.Add("From", from);
		parameters.Add("To", to);

		if (!sql.Contains("Message"))
		{
			// 1. Department Filter (Safe for all reports because they all use 'e' for Employees)
			if (!string.IsNullOrEmpty(deptId) && deptId != "All")
			{
				sql += " AND e.DepartmentId = @DeptId";
			}

			// 2. Date Filters (Only apply to reports that have a transaction date)
			// We EXCLUDE "LeavesNotApplied" and "EmployeeList" here
			var reportsWithDates = new[] { "ExitStaff", "AdvancesApplied", "LeavesApplied", "ApprovedLeaves", "RejectedLeaves" };

			if (reportsWithDates.Contains(reportType))
			{
				if (from.HasValue)
				{
					if (reportType == "ExitStaff") sql += " AND log.EffectiveDate >= @From";
					else if (reportType == "AdvancesApplied") sql += " AND a.RequestDate >= @From";
					else sql += " AND l.FromDate >= @From"; // Catch-all for other Leave reports
				}

				if (to.HasValue)
				{
					if (reportType == "ExitStaff") sql += " AND log.EffectiveDate <= @To";
					else if (reportType == "AdvancesApplied") sql += " AND a.RequestDate <= @To";
					else sql += " AND l.FromDate <= @To";
				}
			}
		}
		var rows = await db.QueryAsync<dynamic>(sql, parameters);
		return MapToDynamicResult(rows);
	}
	//==================================================================================================
	//-------------------------- END OF PUBLIC METHODS ------------------------------------------
	//==================================================================================================

	// EXCEL HELPER TO MAP ANY DTO TO A DYNAMIC RESULT (FOR EXCEL EXPORTS)============================
	public async Task<byte[]> GenerateExcelReportAsync(List<IDictionary<string, object>> data, string reportName)
	{
		// Wrap the synchronous Excel generation in a background task
		return await Task.Run(() =>
		{
			using var workbook = new XLWorkbook();
			var worksheet = workbook.Worksheets.Add("Data");

			if (data == null || !data.Any())
			{
				worksheet.Cell(1, 1).Value = "No records found.";
				using var ms = new MemoryStream();
				workbook.SaveAs(ms);
				return ms.ToArray();
			}

			// 1. Headers
			var columns = data.First().Keys.ToList();
			for (int i = 0; i < columns.Count; i++)
			{
				var cell = worksheet.Cell(1, i + 1);
				cell.Value = columns[i];
				cell.Style.Font.Bold = true;
				cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#3f51b5"); // Professional Blue
				cell.Style.Font.FontColor = XLColor.White;
			}

			// 2. Data
			for (int r = 0; r < data.Count; r++)
			{
				for (int c = 0; c < columns.Count; c++)
				{
					var value = data[r][columns[c]];
					worksheet.Cell(r + 2, c + 1).Value = value?.ToString() ?? "";
				}
			}

			worksheet.Columns().AdjustToContents();

			using var stream = new MemoryStream();
			workbook.SaveAs(stream);
			return stream.ToArray();
		});
	}
	//==================================================================================================
	//-------------------------- END OF EXCEL HELPER ------------------------------------------
	//==================================================================================================
	// Helper to map any IEnumerable<T> to DynamicReportResult NOTE==========IKUWE YA MWISHO============
	private DynamicReportResult MapToDynamicResult(IEnumerable<object> rows)
	{
		var result = new DynamicReportResult();
		var dataList = new List<IDictionary<string, object>>();

		foreach (var row in rows)
		{
			if (row is IDictionary<string, object> dict)
			{
				dataList.Add(dict);
			}
			else
			{
				// This part converts your DTOs (BankStatutoryReportDto, etc.) into a Dictionary
				var dictionary = new Dictionary<string, object>();
				foreach (var prop in row.GetType().GetProperties())
				{
					dictionary[prop.Name] = prop.GetValue(row) ?? "";
				}
				dataList.Add(dictionary);
			}
		}

		if (dataList.Any())
		{
			result.Columns = dataList.First().Keys.ToList();
			result.Data = dataList;
		}
		return result;
	}
}