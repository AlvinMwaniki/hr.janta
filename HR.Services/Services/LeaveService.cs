// HR.Services/Services/LeaveService.cs

using HR.Data; // Your DbContext
using HR.Data.Models.Leaves; // Your LeaveRequest entity
using HR.Services.DTO;
using HR.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
namespace HR.Services.Services;


public class LeaveService : ILeaveService
{
	private readonly HRDbContext _db;
	private readonly ICurrentUserService _currentUser;

	public LeaveService(HRDbContext db, ICurrentUserService currentUser)
	{
		_db = db;
		_currentUser = currentUser;
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

		return true;
	}
}