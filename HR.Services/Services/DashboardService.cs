using HR.Core;
using HR.Core.Enums;
using HR.Data;
using HR.Data.Models.Recruitment;
using HR.Services.DTO;
using HR.Services.Interfaces; 
using HR.Services.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace HR.Services
{
	public class DashboardService
	{
		private readonly IDbContextFactory<HRDbContext> _dbFactory; 
		private readonly ICurrentUserService _currentUser;
		private readonly IMemoryCache _cache;

		public DashboardService(IDbContextFactory<HRDbContext> dbFactory, ICurrentUserService currentUser, IMemoryCache cache)
		{
			_dbFactory = dbFactory; 
			_currentUser = currentUser;
			_cache = cache;
		}

		/*public DashboardService(HRDbContext db)
		{
			_db = db;
		}*/

		public async Task<int> GetTotalEmployeesAsync()
		{
			try
			{
				using var db = await _dbFactory.CreateDbContextAsync();
				return await db.Employees.CountAsync();
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Critical Error in GetTotalEmployees: {ex.Message}");

				return 0;
			}
		}

		public async Task<int> GetTotalDepartmentsAsync()
		{
			try
			{
				using var db = await _dbFactory.CreateDbContextAsync();
				return await db.Departments.CountAsync();
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Critical Error in GetTotalDepartments: {ex.Message}");
				return 0;
			}
			}

		public async Task<int> GetTotalLeavesAsync()
		{
			try
			{
				using var db = await _dbFactory.CreateDbContextAsync();
				return await db.LeaveRequests.CountAsync();
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Critical Error in GetTotalLeaves: {ex.Message}");
				return 0;
			}
		}

		public async Task<int> GetTotalAdvancesAsync()
		{
			try
			{
				using var db = await _dbFactory.CreateDbContextAsync();
				return await db.SalaryAdvances.CountAsync();
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Critical Error in GetTotalAdvances: {ex.Message}");
				return 0;
			}
		}

public async Task<(int working, int onLeave)> GetWorkingVsLeaveAsync(DateTime asOf)
        {
            using var db = await _dbFactory.CreateDbContextAsync();
            var onLeave = await db.LeaveRequests
                .Where(l => l.FromDate.Date <= asOf.Date && l.ToDate.Date >= asOf.Date && l.Status == "Approved")
                .Select(l => l.EmployeeId).Distinct().CountAsync();

            var totalEmployees = await db.Employees.CountAsync();
            return (totalEmployees - onLeave, onLeave);
        }

		public async Task<int> GetEmployeesOnLeaveAsync()
		{
			using var db = await _dbFactory.CreateDbContextAsync();
			var today = DateTime.Today;
			return await db.LeaveRequests
				.Where(l => l.FromDate.Date <= today && l.ToDate.Date >= today && l.Status == "Approved")
				.Select(l => l.EmployeeId).Distinct().CountAsync();
		}

		public async Task<int> GetEmployeesNotOnLeaveAsync()
		{
			using var db = await _dbFactory.CreateDbContextAsync();
			var totalEmployees = await db.Employees.CountAsync();
			var today = DateTime.Today;
			var onLeave = await db.LeaveRequests
				.Where(l => l.FromDate.Date <= today && l.ToDate.Date >= today && l.Status == "Approved")
				.Select(l => l.EmployeeId).Distinct().CountAsync();

			return totalEmployees - onLeave;
		}

		//EMPLOYEE METHODS
		// Define the maximum annual leave days (21 is the example, but this should be configurable)
		private const int AnnualLeaveDays = 21;
		public async Task<int> GetMyAnnualLeaveBalanceAsync()
		{
			// 1. Get the total days taken this year
			var daysTaken = await GetMyLeavesTakenAsync(); // Your existing method handles the lookup

			// 2. Define the limit
			const int annualLimit = HRConstants.AnnualLeaveDaysLimit;

			// 3. Calculate remaining balance
			var balance = annualLimit - daysTaken;

			// Ensure the balance is not negative
			return Math.Max(0, balance);
		}

		public async Task<int> GetMyLeavesTakenAsync()
		{
			var userId = await _currentUser.GetCurrentUserIdAsync();
			if (userId == Guid.Empty) return 0;

			using var db = await _dbFactory.CreateDbContextAsync();
			var employeeId = await db.Employees.Where(e => e.UserId == userId).Select(e => e.Id).FirstOrDefaultAsync();
			if (employeeId == Guid.Empty) return 0;

			var startOfYear = new DateTime(DateTime.Today.Year, 1, 1);
			var endOfYear = new DateTime(DateTime.Today.Year, 12, 31);

			var approvedRequests = await db.LeaveRequests
					.Where(l => l.EmployeeId == employeeId && l.Status == "Approved" && l.FromDate >= startOfYear && l.ToDate <= endOfYear)
					.ToListAsync();

			return approvedRequests.Sum(l => (int)Math.Ceiling((l.ToDate - l.FromDate).TotalDays) + 1);
		}

		public async Task<int> GetMyLeaveBalanceAsync()
		{
			var takenDays = await GetMyLeavesTakenAsync();
			return Math.Max(0, HRConstants.AnnualLeaveDaysLimit - takenDays);
		}

		public async Task<string?> GetMyJobTitleAsync()
		{
			var details = await _currentUser.GetCurrentEmployeeDetailsAsync();

			// The details object is structured to handle the lookup safely.
			return details?.JobTitle ?? "N/A";
		}

		public async Task<string?> GetMyDepartmentAsync()
		{
			var details = await _currentUser.GetCurrentEmployeeDetailsAsync();

			// The details object is structured to handle the lookup safely.
			return details?.DepartmentName ?? "N/A";
		}

		// Inside HR.Services/DashboardService.cs

		public async Task<int> GetMyPendingTasksAsync()
		{
			var userId = await _currentUser.GetCurrentUserIdAsync();
			if (userId == Guid.Empty) return 0;

			using var db = await _dbFactory.CreateDbContextAsync();
			var employeeId = await db.Employees.Where(e => e.UserId == userId).Select(e => e.Id).FirstOrDefaultAsync();
			if (employeeId == Guid.Empty) return 0;

			return await db.LeaveRequests.CountAsync(l => l.EmployeeId == employeeId && l.Status == "Pending");
		}
		/*public async Task<DateTime?> GetMyLastPayslipDateAsync()
		{
			// Get the ID of the current employee
			var employeeId = _currentUser.GetCurrentEmployeeId();
			if (employeeId == Guid.Empty) return null;

			// Assuming you have a Payslips DbSet and a Date field on it
			return await _db.Payslips // Adjust DbSet name if needed
				.Where(p => p.EmployeeId == employeeId)
				.OrderByDescending(p => p.PayDate) // Assuming PayDate is the field
				.Select(p => (DateTime?)p.PayDate)
				.FirstOrDefaultAsync();*/


		//===============CHARTS ADVANCE==================
		public async Task<AdvanceChartDataDto> GetAdvanceStatusChartDataAsync()
		{
			using var db = await _dbFactory.CreateDbContextAsync();
			var requests = await db.SalaryAdvances.AsNoTracking()
				.Where(a => a.RequestDate.Year == DateTime.Today.Year).ToListAsync();

			return new AdvanceChartDataDto
			{
				Approved = requests.Count(a => a.Status == "Approved"),
				PendingOrRejected = requests.Count(a => a.Status != "Approved")
			};
		}


		//===============LEAVE NOTIFICATIONS==================
		// ⭐ NEW: CHART METHOD FOR LEAVE STATUS ⭐
		public async Task<LeaveChartDataDto> GetLeaveStatusChartDataAsync()
		{
			using var db = await _dbFactory.CreateDbContextAsync();
			var year = DateTime.Today.Year;
			var stats = await db.LeaveRequests.Where(l => l.FromDate.Year == year)
				.GroupBy(l => l.Status).Select(g => new { Status = g.Key, Count = g.Count() }).ToListAsync();

			return new LeaveChartDataDto
			{
				LeavesApproved = stats.FirstOrDefault(s => s.Status == "Approved")?.Count ?? 0,
				LeavesPending = stats.FirstOrDefault(s => s.Status == "Pending")?.Count ?? 0,
				LeavesRejected = stats.FirstOrDefault(s => s.Status == "Rejected")?.Count ?? 0
			};
		}
		// NEW MYACIVITY EMPLOYEE DASHBOARD METHOD
		public async Task<List<UnifiedRequestDto>> GetMyRecentApplicationsAsync(int count = 5)
		{
			var userId = await _currentUser.GetCurrentUserIdAsync();
			if (userId == Guid.Empty) return new List<UnifiedRequestDto>();

			using var db = await _dbFactory.CreateDbContextAsync();
			var employeeId = await db.Employees.Where(e => e.UserId == userId).Select(e => e.Id).FirstOrDefaultAsync();
			if (employeeId == Guid.Empty) return new List<UnifiedRequestDto>();

			var leaves = await db.LeaveRequests.Where(l => l.EmployeeId == employeeId)
				.OrderByDescending(l => l.CreatedAt).Take(count)
				.Select(l => new UnifiedRequestDto
				{
					Id = l.Id,
					RequestType = "Leave",
					Description = l.LeaveType,
					Detail = "Time Off Request",
					Date = l.CreatedAt,
					Status = Enum.Parse<LeaveStatus>(l.Status)
				}).ToListAsync();

			var advances = await db.SalaryAdvances.Where(a => a.EmployeeId == employeeId)
				.OrderByDescending(a => a.RequestDate).Take(count)
				.Select(a => new UnifiedRequestDto
				{
					Id = a.Id,
					RequestType = "Advance",
					Description = "Salary Advance",
					Detail = a.Amount.ToString("C"),
					Date = a.RequestDate,
					Status = Enum.Parse<LeaveStatus>(a.Status)
				}).ToListAsync();

			return leaves.Concat(advances).OrderByDescending(x => x.Date).Take(count).ToList();
		}

		//PENDING STAFF APPROVALS
		public async Task<List<UnifiedRequestDto>> GetAllPendingApplicationsAsync()
		{
			using var db = await _dbFactory.CreateDbContextAsync();
			var rawLeaves = await db.LeaveRequests.Include(l => l.Employee).Where(l => l.Status == "Pending")
				.OrderByDescending(l => l.CreatedAt).Select(l => new {
					l.Id,
					l.Employee.FirstName,
					l.Employee.LastName,
					l.LeaveType,
					l.FromDate,
					l.ToDate,
					l.CreatedAt
				}).ToListAsync();

			var leaves = rawLeaves.Select(l => new UnifiedRequestDto
			{
				Id = l.Id,
				RequestType = "Leave",
				Description = $"{l.FirstName} {l.LastName}",
				Detail = $"{l.LeaveType} ({(int)(l.ToDate - l.FromDate).TotalDays + 1} Days)",
				Date = l.CreatedAt == default ? DateTime.Now : l.CreatedAt,
				Status = LeaveStatus.Pending
			}).ToList();

			var advances = await db.SalaryAdvances.Include(a => a.Employee).Where(a => a.Status == "Pending")
				.OrderByDescending(a => a.RequestDate).Select(a => new UnifiedRequestDto
				{
					Id = a.Id,
					RequestType = "Advance",
					Description = $"{a.Employee.FirstName} {a.Employee.LastName}",
					Detail = $"Salary Advance: {a.Amount.ToString("C")}",
					Date = a.RequestDate,
					Status = LeaveStatus.Pending
				}).ToListAsync();

			return leaves.Concat(advances).OrderByDescending(x => x.Date).ToList();

		}
		public async Task<PendingCountDto> GetPendingCountsAsync()
		{
			if (!_cache.TryGetValue("PendingCounts", out PendingCountDto? counts))
			{
				using var db = await _dbFactory.CreateDbContextAsync();
				var leaves = await db.LeaveRequests.CountAsync(l => l.Status == "Pending");
				var advances = await db.SalaryAdvances.CountAsync(a => a.Status == "Pending");
				var jobApps = await db.JobApplications //MPYAA
						   .CountAsync(j => j.Status == ApplicationStatus.New);

				counts = new PendingCountDto
				{
					Leaves = leaves,
					Advances = advances,
					JobApplications = jobApps  
				}; 
				_cache.Set("PendingCounts", counts, TimeSpan.FromMinutes(2));
			}
			return counts ?? new PendingCountDto();
		}

		public class PendingCountDto
		{
			public int Leaves { get; set; }
			public int Advances { get; set; }
			public int JobApplications { get; set; } 
			public int Total => Leaves + Advances + JobApplications;
		}

		public async Task<List<UnifiedRequestDto>> GetJobApplicationsForWidgetAsync()
		{
			using var db = await _dbFactory.CreateDbContextAsync();

			return await db.JobApplications
				.Include(j => j.JobListing)
				.Where(j => j.Status == ApplicationStatus.New)
				.OrderByDescending(j => j.AppliedAt)
				.Select(j => new UnifiedRequestDto
				{
					Id = j.Id,
					RequestType = "JobApp",
					Description = j.FullName,
					Detail = $"Applied for: {j.JobListing!.ExternalTitle} (AI Match: {j.SuitabilityScore}%)",
					Date = j.AppliedAt,
					Status = LeaveStatus.Pending // Required for the Widget's Enum check
				}).ToListAsync();
		}

	}

}
