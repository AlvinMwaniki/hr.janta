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
								// NOTE: Ensure you add your LeaveType filter here if needed, e.g.,
								// l.LeaveType == HR.Core.Enums.LeaveType.Annual.ToString() && 
								l.FromDate >= startOfYear &&
								l.ToDate <= endOfYear)
					// CRITICAL: Pull the list into memory before calculating the sum!
					.ToListAsync();

			// ⭐ FIX: 2. Calculate the sum in C# memory (Client side) ⭐
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
			// ⭐ FIX: Use the existing, reliable details method ⭐
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

			// ⭐ FIX: Return a tuple directly ⭐
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
			// Query all leave requests in the database for the Admin view.
			var requests = await _db.LeaveRequests
				.AsNoTracking()
                .Where(l => l.FromDate.Year == DateTime.Today.Year)
                .ToListAsync();

			// Group and count the requests by status
			var approved = requests.Count(l => l.Status == "Approved");
			var pending = requests.Count(l => l.Status == "Pending");
			var rejected = requests.Count(l => l.Status == "Rejected");

			return new LeaveChartDataDto
			{
				LeavesApproved = approved,
				LeavesPending = pending,
				LeavesRejected = rejected
			};
		}

	}
}
