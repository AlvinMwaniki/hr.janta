// HR.Services/Services/LeaveService.cs
using HR.Core.Enums;
using HR.Data; // Your DbContext
using HR.Data.Models.Leaves; // Your LeaveRequest entity
using HR.Services.DTO;
using HR.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

using System;

using System.Threading.Tasks;

namespace HR.Services.Services;





public class LeaveService : ILeaveService

{

	private readonly HRDbContext _db;

	private readonly ICurrentUserService _currentUser;
	private readonly RefreshBroker _broker;
	private readonly ILeaveNotificationService _notificationService;
	private readonly IMemoryCache _cache;
	public LeaveService(HRDbContext db, ICurrentUserService currentUser, ILeaveNotificationService notificationService, IMemoryCache cache, RefreshBroker broker)

	{

		_db = db;

		_currentUser = currentUser;

		_notificationService = notificationService;
		_cache = cache;
		_broker = broker;
	}

	// 1. Fetching Pending Requests for Admin

	public async Task<List<LeaveRequestViewDto>> GetPendingRequestsAsync()
	{
		// 1. Fetch raw data from DB first (Simple columns only)
		var pendingData = await _db.LeaveRequests
			.Include(l => l.Employee)
			  .ThenInclude(e => e.Department)
			.Where(l => l.Status == "Pending")
			.OrderByDescending(l => l.FromDate)
			.Select(l => new
			{
				l.Id,
				FirstName = l.Employee.FirstName,
				MiddleName = l.Employee.MiddleName,
				LastName = l.Employee.LastName,
				JobTitle = l.Employee.JobTitle,           // Added JobTitle
				DepartmentName = l.Employee.Department.Name,
				l.LeaveType,
				l.FromDate,
				l.ToDate,
				l.Reason,
				l.Status
			})
			.ToListAsync();

		// 2. Map to DTO in memory (C# handles the math and Enums here)
		return pendingData.Select(l => new LeaveRequestViewDto
		{
			Id = l.Id,
			EmployeeName = $"{l.FirstName} {l.MiddleName} {l.LastName}".Trim(),
			EmployeeJobTitle = l.JobTitle ?? "Staff",      // Map to DTO property
			Department = l.DepartmentName ?? "General",    // Map to DTO property
			LeaveType = Enum.TryParse<LeaveType>(l.LeaveType, out var type) ? type : LeaveType.Annual,
			FromDate = l.FromDate,
			ToDate = l.ToDate,
			Reason = l.Reason,
			Status = Enum.TryParse<LeaveStatus>(l.Status, out var status) ? status : LeaveStatus.Pending,
			// Math is now executed by the C# engine, preventing the Coercion error
			DurationInDays = (int)(l.ToDate - l.FromDate).TotalDays + 1
		}).ToList();
	}



	// 3. Employee Canceling their own request

	public async Task<bool> CancelLeaveRequestAsync(Guid leaveId)

	{

		var request = await _db.LeaveRequests.FindAsync(leaveId);

		if (request == null || request.Status != "Pending") return false;



		request.Status = LeaveStatus.Cancelled.ToString();

		await _db.SaveChangesAsync();
		_cache.Remove("PendingCounts");
		return true;

	}

	public async Task<bool> SubmitLeaveRequestAsync(LeaveSubmissionDto dto)

	{

		// 1. Resolve Identity: Get the reliable User ID

		var userId = await _currentUser.GetCurrentUserIdAsync();

		if (userId == Guid.Empty)

		{

			throw new InvalidOperationException("User session is invalid. Please log in again.");

		}



		// 2. Resolve EmployeeId: Lookup in the database

		var employee = await _db.Employees

		.AsNoTracking()

		.Where(e => e.UserId == userId)

			.Select(e => new { e.Id, e.AnnualLeaveBalanceDays })

		.FirstOrDefaultAsync();



		if (employee == null)

		{

			throw new InvalidOperationException("No employee record found linked to your user account. Cannot submit leave.");

		}

		var startDate = dto.StartDate!.Value.Date;

		var endDate = dto.EndDate!.Value.Date;

		var today = DateTime.Today;

		// 3. Validation (Add your full date and business validation here)

		if (startDate < today || endDate < today)

		{

			throw new InvalidOperationException("Leave dates cannot be in the past.");

		}



		// Calculate required days (Dto has this as a computed property)

		int requiredDays = dto.DurationInDays;

		int availableBalance = employee.AnnualLeaveBalanceDays;



		if (requiredDays <= 0)

		{

			throw new InvalidOperationException("Leave duration must be at least one day.");

		}



		if (requiredDays > availableBalance)

		{

			throw new InvalidOperationException($"Insufficient leave balance. You are requesting {requiredDays} days, but only have {availableBalance} days remaining.");

		}



		if (dto.StartDate == null || dto.EndDate == null || dto.StartDate.Value > dto.EndDate.Value)

		{

			throw new InvalidOperationException("Start Date must be before or equal to End Date.");

		}



		// 4. Map DTO to Entity

		var leaveRequest = new LeaveRequest

		{

			Id = Guid.NewGuid(),

			EmployeeId = employee.Id, // ⭐ THE SUCCESSFUL LINK ⭐

			LeaveType = dto.Type.ToString(), // Converts Enum to string as per your entity

			FromDate = dto.StartDate.Value,

			ToDate = dto.EndDate.Value,

			Reason = dto.Reason,

			Status = "Pending"

			// ApprovedByUserId is null by default

		};



		_db.LeaveRequests.Add(leaveRequest);

		await _db.SaveChangesAsync();

		_cache.Remove("PendingCounts");

		try
		{
			// Fetch the name of the person who just applied
			var empName = await _db.Employees
				.Where(e => e.Id == employee.Id)
				.Select(e => $"{e.FirstName} {e.LastName}")
				.FirstOrDefaultAsync();

			await _broker.NotifyNewRequest("Leave Request", empName ?? "An Employee");
		}
		catch { /* Prevent notification failure from breaking the save */ }

		return true;
	}



	public async Task<bool> ReviewLeaveRequestAsync(Guid leaveId, LeaveStatus newStatus, string? comment = null)

	{

		var request = await _db.LeaveRequests.Include(l => l.Employee).FirstOrDefaultAsync(x => x.Id == leaveId);

		if (request == null) return false;



		// Record who is approving (Admin)

		var adminId = await _currentUser.GetCurrentUserIdAsync();



		request.Status = newStatus.ToString();

		// In a real system, you'd have 'ApprovedBy' and 'ReviewComment' fields in your DB

		request.ApprovedByUserId = await _currentUser.GetCurrentUserIdAsync();



		// If approved, deduct leave balance

		if (newStatus == LeaveStatus.Approved)

		{

			int days = (int)(request.ToDate - request.FromDate).TotalDays + 1;

			request.Employee.AnnualLeaveBalanceDays -= days;

		}

		await _db.SaveChangesAsync();

		// Notify Employee via SignalR
		_cache.Remove("PendingCounts");
		await _broker.CallRequestChanged();

		var employeeUserId = request.Employee.UserId.ToString();



		await _notificationService.NotifyLeaveUpdateAsync(employeeUserId, $"Your leave request has been {newStatus}");

		return true;

	}

	public async Task<List<LeaveRequestViewDto>> GetMyLeaveRequestsAsync()

	{

		var userId = await _currentUser.GetCurrentUserIdAsync();
		// STEP 1: Fetch the data from the database (Simple columns only)
		var rawData = await _db.LeaveRequests
			.Where(l => l.Employee.UserId == userId)
			.OrderByDescending(l => l.CreatedAt)
			.Select(l => new
			{
				l.Id,
				l.LeaveType, // String in DB
				l.FromDate,  // DateTime
				l.ToDate,    // DateTime
				l.Status,    // String in DB
				l.CreatedAt,
				l.Reason
			})
			.ToListAsync();

		// STEP 2: Map to your DTO using C# (where math and Enums work perfectly)
		return rawData.Select(l => new LeaveRequestViewDto
		{
			Id = l.Id,
			FromDate = l.FromDate,
			ToDate = l.ToDate,
			CreatedAt = l.CreatedAt,
			Reason = l.Reason ?? string.Empty,

			// C# Math: Safe from "Coercion" errors
			DurationInDays = (int)(l.ToDate - l.FromDate).TotalDays + 1,

			// Safe Enum Parsing
			LeaveType = Enum.TryParse<LeaveType>(l.LeaveType, out var type) ? type : LeaveType.Annual,
			Status = Enum.TryParse<LeaveStatus>(l.Status, out var status) ? status : LeaveStatus.Pending
		}).ToList();

	}

	public async Task<List<LeaveRequestViewDto>> GetAllRequestsAsync()
	{
		var allData = await _db.LeaveRequests
			.Include(l => l.Employee)
				.ThenInclude(e => e.Department) // CRITICAL: This pulls Department Name
			.OrderByDescending(l => l.CreatedAt)
			.Select(l => new
			{
				l.Id,
				l.EmployeeId,
				FirstName = l.Employee.FirstName,
				LastName = l.Employee.LastName,
				JobTitle = l.Employee.JobTitle, // From Employee Table
				DepartmentName = l.Employee.Department.Name, // From Department Table
				l.LeaveType,
				l.FromDate,
				l.ToDate,
				l.Reason,
				l.Status,
				l.CreatedAt
			})
			.ToListAsync();

		return allData.Select(l => new LeaveRequestViewDto
		{
			Id = l.Id,
			EmployeeName = $"{l.FirstName} {l.LastName}",
			EmployeeJobTitle = l.JobTitle ?? "Staff",
			Department = l.DepartmentName ?? "General", 
			LeaveType = Enum.TryParse<LeaveType>(l.LeaveType, out var type) ? type : LeaveType.Annual,
			FromDate = l.FromDate,
			ToDate = l.ToDate,
			Reason = l.Reason ?? string.Empty,
			Status = Enum.TryParse<LeaveStatus>(l.Status, out var status) ? status : LeaveStatus.Pending,
			DurationInDays = (int)(l.ToDate - l.FromDate).TotalDays + 1
		}).ToList();
	}
}
