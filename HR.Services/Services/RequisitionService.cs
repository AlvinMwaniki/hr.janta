using DocumentFormat.OpenXml.InkML;

using HR.Core.Enums;
using HR.Data;
using HR.Data.Models.Recruitment;
using HR.Services.DTO;
using HR.Services.DTO.Recruitment;
using HR.Services.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace HR.Services.Services
{
	public class RequisitionService : IRequisitionService
	{
		private readonly HRDbContext _db;
		private readonly ICurrentUserService _currentUser;

		private const int MaxApprovalLevel = 1; // WILL CHANGE THIS IN FUTRE INCASE ANYONE WANTS MANY APPROVALS

		public RequisitionService(HRDbContext db, ICurrentUserService currentUser)
		{
			_db = db;
			_currentUser = currentUser;
		}

		// ✅ CREATE
		public async Task<bool> CreateAsync(RequisitionCreateDto dto)
		{
			var userId = await _currentUser.GetCurrentUserIdAsync();
			if (userId == Guid.Empty)
				throw new InvalidOperationException("Invalid session.");
			if (dto.DepartmentId == Guid.Empty)
				throw new InvalidOperationException("Please select a valid department.");

			var requisition = new JobRequisition
			{
				Id = Guid.NewGuid(),
				RequisitionNumber = $"REQ-{DateTime.UtcNow:yyyyMMddHHmmss}",
				JobTitle = dto.JobTitle,
				DepartmentId = dto.DepartmentId,
				ContractType = dto.ContractType,
				SalaryMin = dto.SalaryMin,
				SalaryMax = dto.SalaryMax,
				Description = dto.Description,
				RequestedByUserId = userId,
				RequiredSkills = dto.RequiredSkills,
				RequiredExperienceYears = dto.RequiredExperienceYears,
				RequiredEducationLevel = dto.RequiredEducationLevel,

				Status = RequisitionStatus.Draft,
				CreatedAt = DateTime.UtcNow
			};

			_db.JobRequisition.Add(requisition);
			await _db.SaveChangesAsync();

			return true;
		}

		// ✅ SUBMIT (Creates Level 1 Approval)
		public async Task<bool> SubmitAsync(Guid requisitionId)
		{
			var requisition = await _db.JobRequisition
				.Include(r => r.Approvals)
				.FirstOrDefaultAsync(r => r.Id == requisitionId);

			if (requisition == null) return false;

			if (requisition.Status != RequisitionStatus.Draft)
				throw new InvalidOperationException("Only Draft requisitions can be submitted.");

			requisition.Status = RequisitionStatus.PendingApproval;

			var approval = new RequisitionApproval
			{
				Id = Guid.NewGuid(),
				JobRequisitionId = requisition.Id,
				ApprovalLevel = 1,
				Status = ApprovalStatus.Pending
			};

			_db.RequisitionApprovals.Add(approval);

			await _db.SaveChangesAsync();
			return true;
		}

		// ✅ APPROVE (Multi-Level Logic)
		public async Task<bool> ApproveAsync(Guid requisitionId)
		{
			var userId = await _currentUser.GetCurrentUserIdAsync();

			var requisition = await _db.JobRequisition
				.Include(r => r.Approvals)
				.FirstOrDefaultAsync(r => r.Id == requisitionId);

			if (requisition == null) return false;

			var currentApproval = requisition.Approvals
				.Where(a => a.Status == ApprovalStatus.Pending)
				.OrderBy(a => a.ApprovalLevel)
				.FirstOrDefault();

			if (currentApproval == null)
				throw new InvalidOperationException("No pending approval found.");

			// Mark current level approved
			currentApproval.Status = ApprovalStatus.Approved;
			currentApproval.ActionByUserId = userId;
			currentApproval.ActionDate = DateTime.UtcNow;

			// If final level
			if (currentApproval.ApprovalLevel >= MaxApprovalLevel)
			{
				requisition.Status = RequisitionStatus.Approved;
				requisition.ApprovedAt = DateTime.UtcNow;
				requisition.BudgetApproved = true;
			}
			else
			{
				// Create next level approval
				var nextApproval = new RequisitionApproval
				{
					Id = Guid.NewGuid(),
					JobRequisitionId = requisition.Id,
					ApprovalLevel = currentApproval.ApprovalLevel + 1,
					Status = ApprovalStatus.Pending
				};

				_db.RequisitionApprovals.Add(nextApproval);
			}

			await _db.SaveChangesAsync();
			return true;
		}

		// ✅ REJECT (Stops Workflow Immediately)
		public async Task<bool> RejectAsync(Guid requisitionId, string comment)
		{
			var userId = await _currentUser.GetCurrentUserIdAsync();

			var requisition = await _db.JobRequisition
				.Include(r => r.Approvals)
				.FirstOrDefaultAsync(r => r.Id == requisitionId);

			if (requisition == null) return false;

			var currentApproval = requisition.Approvals
				.Where(a => a.Status == ApprovalStatus.Pending)
				.OrderBy(a => a.ApprovalLevel)
				.FirstOrDefault();

			if (currentApproval == null)
				throw new InvalidOperationException("No pending approval found.");

			currentApproval.Status = ApprovalStatus.Rejected;
			currentApproval.ActionByUserId = userId;
			currentApproval.ActionDate = DateTime.UtcNow;
			currentApproval.Comments = comment;

			requisition.Status = RequisitionStatus.Rejected;

			await _db.SaveChangesAsync();
			return true;
		}

		// ✅ GET ALL
		public async Task<List<RequisitionViewDto>> GetAllAsync()
		{
			return await _db.JobRequisition
				.Include(r => r.Department)
				.Include(r => r.Approvals)
				.Select(r => new RequisitionViewDto
				{
					Id = r.Id,
					RequisitionNumber = r.RequisitionNumber,
					JobTitle = r.JobTitle,
					DepartmentId = r.DepartmentId,
					DepartmentName = r.Department!.Name,
					ContractType = r.ContractType,
					SalaryMin = r.SalaryMin,
					SalaryMax = r.SalaryMax,
					Description = r.Description,
					RequiredSkills = r.RequiredSkills,
					RequiredExperienceYears = r.RequiredExperienceYears,
					RequiredEducationLevel = r.RequiredEducationLevel,

					Status = r.Status,
					CreatedAt = r.CreatedAt,

					// ✅ Proper projection
					Approvals = r.Approvals
						.OrderBy(a => a.ApprovalLevel)
						.Select(a => new RequisitionApprovalDto
						{
							ApprovalLevel = a.ApprovalLevel,
							Status = a.Status.ToString(),
							Comments = a.Comments,
							ActionDate = a.ActionDate,
							ActionByUserId = a.ActionByUserId
						}).ToList()
				})
				.ToListAsync();
		}


		public async Task<List<RequisitionViewDto>> GetMyRequisitionsAsync()
		{
			var userId = await _currentUser.GetCurrentUserIdAsync();

			return await _db.JobRequisition
				.Include(r => r.Department)
				.Where(r => r.RequestedByUserId == userId)
				.Select(r => new RequisitionViewDto
				{
					Id = r.Id,
					RequisitionNumber = r.RequisitionNumber,
					JobTitle = r.JobTitle,
					DepartmentId = r.DepartmentId,
					DepartmentName = r.Department!.Name,
					ContractType = r.ContractType,
					SalaryMin = r.SalaryMin,
					SalaryMax = r.SalaryMax,
					Description = r.Description,
					Status = r.Status,
					CreatedAt = r.CreatedAt
				})
				.ToListAsync();
		}
		public async Task<List<DepartmentDto>> GetDepartmentsAsync()
		{
			return await _db.Departments
				.Select(d => new DepartmentDto { Id = d.Id, Name = d.Name })
				.ToListAsync();
		}
		public async Task<RequisitionViewDto?> GetByIdAsync(Guid id)
		{
			return await _db.JobRequisition
				.Include(r => r.Department)
				.Include(r => r.Approvals)
		       	.ThenInclude(a => a.ActionByUser)
				.OrderByDescending(r => r.CreatedAt)
				.Where(r => r.Id == id)
				.Select(r => new RequisitionViewDto
				{
					Id = r.Id,
					RequisitionNumber = r.RequisitionNumber,
					JobTitle = r.JobTitle,
					DepartmentId = r.DepartmentId,
					DepartmentName = r.Department!.Name,
					ContractType = r.ContractType,
					SalaryMin = r.SalaryMin,
					SalaryMax = r.SalaryMax,
					Description = r.Description,
					RequiredSkills = r.RequiredSkills,
					RequiredExperienceYears = r.RequiredExperienceYears,
					RequiredEducationLevel = r.RequiredEducationLevel,
					Status = r.Status,
					CreatedAt = r.CreatedAt,

					Approvals = r.Approvals
						.OrderBy(a => a.ApprovalLevel)
						.Select(a => new RequisitionApprovalDto
						{
							ApprovalLevel = a.ApprovalLevel,
							Status = a.Status.ToString(),
							Comments = a.Comments,
							ActionDate = a.ActionDate,
							ActionByUserId = a.ActionByUserId,
							ActionByName = a.ActionByUser != null
							? a.ActionByUser.Username
										: null

						}).ToList()
				})
				.FirstOrDefaultAsync();
		}
		public async Task<bool> DeleteAsync(Guid requisitionId)
		{
			var requisition = await _db.JobRequisition.FindAsync(requisitionId);

			if (requisition == null) return false;

			// Optional: Only allow deleting if it's NOT already approved
			// if (requisition.Status == RequisitionStatus.Approved) 
			// throw new InvalidOperationException("Cannot delete an approved requisition.");

			_db.JobRequisition.Remove(requisition);
			await _db.SaveChangesAsync();
			return true;
		}

		public async Task<bool> PublishJobAsync(Guid requisitionId, string externalTitle, string location)
		{
			// 1. Fetch the approved requisition
			var requisition = await _db.JobRequisition
				.FirstOrDefaultAsync(r => r.Id == requisitionId);

			if (requisition == null || requisition.Status != RequisitionStatus.Approved)
				return false;

			// 2. Create the Job Listing (The public advertisement)
			var listing = new JobListing
			{
				Id = Guid.NewGuid(),
				JobRequisitionId = requisition.Id,
				ExternalTitle = externalTitle,
				ExternalDescription = requisition.Description, // Public description
				Location = location,
				IsActive = true,
				PublishedAt = DateTime.UtcNow,
				 RequiredSkills = requisition.RequiredSkills,
				RequiredExperienceYears = requisition.RequiredExperienceYears,
				RequiredEducationLevel = requisition.RequiredEducationLevel

			};

			_db.JobListings.Add(listing); // Make sure 'JobListings' is in your HRDbContext
			await _db.SaveChangesAsync();

			return true;
		}
		public async Task<List<JobListing>> GetJobListingsAsync()
		{
			return await _db.JobListings
				.Include(l => l.JobRequisition) 
				.OrderByDescending(l => l.PublishedAt)
				.ToListAsync();
		}
		public async Task<bool> ToggleListingStatusAsync(Guid listingId)
		{
			var listing = await _db.JobListings.FindAsync(listingId);
			if (listing == null) return false;

			// Flip the status: if true, make false. If false, make true.
			listing.IsActive = !listing.IsActive;

			await _db.SaveChangesAsync();
			return true;
		}

		public async Task<PublicJobDto?> GetPublicJobByIdAsync(Guid listingId)
		{
			return await _db.JobListings
				.Include(l => l.JobRequisition)
				.Where(l => l.Id == listingId && l.IsActive)
				.Select(l => new PublicJobDto
				{
					ListingId = l.Id,
					Title = l.ExternalTitle ?? l.JobRequisition!.JobTitle,
					Location = !string.IsNullOrWhiteSpace(l.Location) ? l.Location : "Headquarters",
					Description = l.ExternalDescription ?? l.JobRequisition!.Description,
					ContractType = l.JobRequisition!.ContractType.ToString(),
					PublishedAt = l.PublishedAt
				})
				.FirstOrDefaultAsync();
		}
	}


}
