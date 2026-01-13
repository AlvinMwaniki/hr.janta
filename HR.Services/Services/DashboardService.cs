using HR.Core;
using HR.Core.Enums;
using HR.Data;
using HR.Services.DTO;
using HR.Services.Interfaces; 
using HR.Services.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace HR.Services
{
	public class DashboardService
	{
		private readonly HRDbContext _db;
		private readonly ICurrentUserService _currentUser;
		public DashboardService(HRDbContext db, ICurrentUserService currentUser)
		{
			_db = db;
			_currentUser = currentUser;
		}

		/*public DashboardService(HRDbContext db)
		{
			_db = db;
		}*/

		public async Task<int> GetTotalEmployeesAsync()
		{
			try
			{
				return await _db.Employees.CountAsync();
			}
			catch
			{
				return 0; // fallback
			}
		}

		public async Task<int> GetTotalDepartmentsAsync()
		{
			try
			{
				return await _db.Departments.CountAsync();
			}
			catch
			{
				return 0; // fallback
			}
		}

		public async Task<int> GetTotalLeavesAsync()
		{
			try
			{
				return await _db.LeaveRequests.CountAsync();
			}
			catch
			{
				return 0; // fallback
			}
		}

		public async Task<int> GetTotalAdvancesAsync()
		{
			try
			{
				return await _db.SalaryAdvances.CountAsync();
			}
			catch
			{
				return 0; // fallback
			}
		}

		public async Task<(int working, int onLeave)> GetWorkingVsLeaveAsync(DateTime asOf)
		{
			var onLeave = await _db.LeaveRequests
	.Where(l => l.FromDate.Date <= asOf.Date &&
				l.ToDate.Date >= asOf.Date &&
				l.Status == "Approved")
	.Select(l => l.EmployeeId)
	.Distinct()
	.CountAsync();



			var totalEmployees = await _db.Employees.CountAsync();

			return (totalEmployees - onLeave, onLeave);
		}

		public async Task<int> GetEmployeesOnLeaveAsync()
		{
			var today = DateTime.Today;

			var onLeave = await _db.LeaveRequests
				.Where(l => l.FromDate.Date <= today &&
							l.ToDate.Date >= today &&
							l.Status == "Approved")
				.Select(l => l.EmployeeId)
				.Distinct()
				.CountAsync();

			return onLeave;
		}

		public async Task<int> GetEmployeesNotOnLeaveAsync()
		{
			var totalEmployees = await _db.Employees.CountAsync();
			var onLeave = await GetEmployeesOnLeaveAsync();

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
			// Get the ID of the current employee
			var userId = await _currentUser.GetCurrentUserIdAsync();
			if (userId == Guid.Empty) return 0;

			var employeeId = await _db.Employees
				.Where(e => e.UserId == userId)
				.Select(e => e.Id)
				.FirstOrDefaultAsync();

			if (employeeId == Guid.Empty) return 0;

			var startOfYear = new DateTime(DateTime.Today.Year, 1, 1);
			var endOfYear = new DateTime(DateTime.Today.Year, 12, 31);

			var approvedRequests = await _db.LeaveRequests
					.Where(l => l.EmployeeId == employeeId &&
								l.Status == "Approved" &&
								// l.LeaveType == HR.Core.Enums.LeaveType.Annual.ToString() && 
								l.FromDate >= startOfYear &&
								l.ToDate <= endOfYear)
					//  Pull the list into memory before calculating the sum!
					.ToListAsync();

			// Use the Enumerable.Sum extension method, which is the C# in-memory version.
			var totalDaysTaken = approvedRequests.Sum(l =>
				(int)Math.Ceiling((l.ToDate - l.FromDate).TotalDays) + 1
			);
			return totalDaysTaken;
		}

		public async Task<int> GetMyLeaveBalanceAsync()
		{
			var takenDays = await GetMyLeavesTakenAsync();

			const int annualLimit = HRConstants.AnnualLeaveDaysLimit;

			// Logic: Balance = Max Annual Days - Days Taken
			return Math.Max(0, annualLimit - takenDays); // Math.Max to prevent negative balance
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
			// ⭐ STEP 1: Get the reliable User ID ⭐
			var userId = await _currentUser.GetCurrentUserIdAsync();
			if (userId == Guid.Empty) return 0;

			// ⭐ STEP 2: Find the Employee ID by querying the database ⭐
			var employeeId = await _db.Employees
				.Where(e => e.UserId == userId)
				.Select(e => e.Id)
				.FirstOrDefaultAsync();

			if (employeeId == Guid.Empty) return 0;

			// Assuming pending tasks are things like leave requests awaiting approval
			var pendingCount = await _db.LeaveRequests
				.CountAsync(l => l.EmployeeId == employeeId &&
								 l.Status == "Pending"); // <-- Uses the found Employee ID

			return pendingCount;
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
			// For simplicity, we query ALL advances in the database for the Admin view.
			var requests = await _db.SalaryAdvances
				.AsNoTracking()
				.Where(a => a.RequestDate.Year == DateTime.Today.Year) // Filter by current year
				.ToListAsync();

			var approved = requests.Count(a => a.Status == "Approved");
			// Combine all non-approved statuses into one category
			var pendingOrRejected = requests.Count(a => a.Status != "Approved");

			//  Return a tuple directly 
			return new AdvanceChartDataDto
			{
				Approved = approved,
				PendingOrRejected = pendingOrRejected
			};
		}


		//===============LEAVE NOTIFICATIONS==================
		// ⭐ NEW: CHART METHOD FOR LEAVE STATUS ⭐
		public async Task<LeaveChartDataDto> GetLeaveStatusChartDataAsync()
		{
			var year = DateTime.Today.Year;

			// Use a single query to get all counts at once for performance
			var stats = await _db.LeaveRequests
				.Where(l => l.FromDate.Year == year)
				.GroupBy(l => l.Status)
				.Select(g => new { Status = g.Key, Count = g.Count() })
				.ToListAsync();

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

			// 1. Get Employee ID (Reusable logic from your other methods)
			var employeeId = await _db.Employees
				.Where(e => e.UserId == userId)
				.Select(e => e.Id)
				.FirstOrDefaultAsync();

			if (employeeId == Guid.Empty) return new List<UnifiedRequestDto>();

			// 2. Fetch Leaves
			var leaves = await _db.LeaveRequests
				.Where(l => l.EmployeeId == employeeId)
				.OrderByDescending(l => l.CreatedAt)
				.Take(count)
				.Select(l => new UnifiedRequestDto
				{
					Id = l.Id,
					RequestType = "Leave",
					Description = l.LeaveType, // e.g., "Annual"
					Detail = "Time Off Request",
					Date = l.CreatedAt,
					Status = Enum.Parse<LeaveStatus>(l.Status)
				}).ToListAsync();

			// 3. Fetch Salary Advances
			var advances = await _db.SalaryAdvances
				.Where(a => a.EmployeeId == employeeId)
				.OrderByDescending(a => a.RequestDate)
				.Take(count)
				.Select(a => new UnifiedRequestDto
				{
					Id = a.Id,
					RequestType = "Advance",
					Description = "Salary Advance",
					Detail = a.Amount.ToString("C"), // Formats as Currency
					Date = a.RequestDate,
					Status = Enum.Parse<LeaveStatus>(a.Status)
				}).ToListAsync();

			// 4. Merge, Sort, and Return
			return leaves.Concat(advances)
				.OrderByDescending(x => x.Date)
				.Take(count)
				.ToList();
		}

		//PENDING STAFF APPROVALS
		public async Task<List<UnifiedRequestDto>> GetAllPendingApplicationsAsync()
		{
			// 1. Fetch all Pending Leaves (Raw Data)
			var rawLeaves = await _db.LeaveRequests
				.Include(l => l.Employee)
				.Where(l => l.Status == "Pending")
				.OrderByDescending(l => l.CreatedAt)
				.Select(l => new
				{
					l.Id,
					l.Employee.FirstName,
					l.Employee.LastName,
					l.LeaveType,
					l.FromDate,
					l.ToDate,
					l.CreatedAt
				}).ToListAsync();

			// Map Leaves to Unified DTO in memory
			var leaves = rawLeaves.Select(l => new UnifiedRequestDto
			{
				Id = l.Id,
				RequestType = "Leave",
				Description = $"{l.FirstName} {l.LastName}",
				// Math and String formatting happen here in C#, avoiding the error
				Detail = $"{l.LeaveType} ({(int)(l.ToDate - l.FromDate).TotalDays + 1} Days)",
				Date = l.CreatedAt == default ? DateTime.Now : l.CreatedAt,
				Status = LeaveStatus.Pending
			}).ToList();

			// 2. Fetch all Pending Advances
			var advances = await _db.SalaryAdvances
				.Include(a => a.Employee)
				.Where(a => a.Status == "Pending")
				.OrderByDescending(a => a.RequestDate)
				.Select(a => new UnifiedRequestDto
				{
					Id = a.Id,
					RequestType = "Advance",
					Description = $"{a.Employee.FirstName} {a.Employee.LastName}",
					Detail = $"Salary Advance: {a.Amount.ToString("C")}",
					Date = a.RequestDate,
					Status = LeaveStatus.Pending
				}).ToListAsync();

			// 3. Merge and Sort
			return leaves.Concat(advances)
				.OrderByDescending(x => x.Date)
				.ToList();
		}
		public async Task<PendingCountDto> GetPendingCountsAsync()
		{
			var leaves = await _db.LeaveRequests.CountAsync(l => l.Status == "Pending");
			var advances = await _db.SalaryAdvances.CountAsync(a => a.Status == "Pending");

			return new PendingCountDto
			{
				Leaves = leaves,
				Advances = advances
			};
		}
		public class PendingCountDto
		{
			public int Leaves { get; set; }
			public int Advances { get; set; }
		}
	}

}
