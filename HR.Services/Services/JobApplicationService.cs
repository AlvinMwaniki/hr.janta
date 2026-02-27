using HR.Core.Enums;
using HR.Data;
using HR.Data.Models.Recruitment;
using HR.Services.DTO;
using HR.Services.Interfaces;

using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HR.Services.Services
{
	public class JobApplicationService : IJobApplicationService
	{
		private readonly IDbContextFactory<HRDbContext> _dbFactory;
		private readonly IAppNotificationService _notificationService;
		private readonly IAtsScoringService _atsScorer;
		private readonly string _uploadPath;
		private readonly ILogger<JobApplicationService> _logger;

		public JobApplicationService(
			IDbContextFactory<HRDbContext> dbFactory,
			IAppNotificationService notificationService,
			IAtsScoringService atsScorer,
			ILogger<JobApplicationService> logger)
		{
			_dbFactory = dbFactory;
			_notificationService = notificationService;
			_atsScorer = atsScorer;
			_logger = logger;

			_uploadPath = Path.Combine(
				Directory.GetCurrentDirectory(),
				"wwwroot",
				"uploads",
				"cvs");

			if (!Directory.Exists(_uploadPath))
				Directory.CreateDirectory(_uploadPath);
		}

		public async Task<bool> SubmitApplicationAsync(
			JobApplication application,
			IBrowserFile file)
		{
			using var db = await _dbFactory.CreateDbContextAsync();

			try
			{
				// 1️⃣ Load Job Listing
				var listing = await db.JobListings
					.AsNoTracking()
					.FirstOrDefaultAsync(j => j.Id == application.JobListingId);

				if (listing == null)
					throw new Exception("Invalid Job Listing.");

				// 2️⃣ Save CV File
				var fileName = $"{Guid.NewGuid()}_{file.Name}";
				var fullPath = Path.Combine(_uploadPath, fileName);

				using (var stream = new FileStream(fullPath, FileMode.Create))
				{
					await file.OpenReadStream(10 * 1024 * 1024)
							  .CopyToAsync(stream);
				}

				application.CVPath = $"/uploads/cvs/{fileName}";
				application.Id = Guid.NewGuid();
				application.AppliedAt = DateTime.UtcNow;

				/* 3️⃣ Parse CV
				byte[] pdfBytes;
				using (var ms = new MemoryStream())
				{
					await file.OpenReadStream().CopyToAsync(ms);
					pdfBytes = ms.ToArray();
				}

				var parsed = await _cvParser.ParseAsync(
					pdfBytes,
					listing.ExternalDescription ?? "",
					listing.ExternalTitle ?? ""
				);

				// 4️⃣ Map Parsed Data (Only if fields empty)
				MapParsedData(application, parsed);*/

				// 5️⃣ Score Application
				var scoring = await _atsScorer
					.ScoreAsync(application, listing);

				application.SuitabilityScore = scoring.Score;
				application.AIRankingReason = scoring.Reason;
				application.SkillsFound = string.Join(",", scoring.MatchedSkills);

				// 6️⃣ Persist
				db.JobApplications.Add(application);
				await db.SaveChangesAsync();

				// 7️⃣ Notify
				await _notificationService.NotifyChangeAsync();

				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Application submission failed.");
				return false;
			}
		}

		/*	private void MapParsedData(
				JobApplication application,
				CvAnalysisResult parsed)
			{
				// Map Education
				if (parsed.Education?.Any() == true)
				{
					application.Education = parsed.Education
						.Select(e => new ApplicantEducation
						{
							Id = Guid.NewGuid(),
							JobApplicationId = application.Id,
							Institution = e.Institution,
							Field = e.Field,
							Level = e.Level,
							StartDate = e.StartDate ?? DateTime.UtcNow,
							EndDate = e.EndDate
						})
						.ToList();
				}

				// Map Experience
				if (parsed.Experience?.Any() == true)
				{
					application.Experience = parsed.Experience
						.Select(ex => new ApplicantExperience
						{
							Id = Guid.NewGuid(),
							JobApplicationId = application.Id,
							Company = ex.Company,
							JobTitle = ex.JobTitle,
							Responsibilities = ex.Responsibilities,
							StartDate = ex.StartDate ?? DateTime.UtcNow,
							EndDate = ex.EndDate
						})
						.ToList();
				}
			}*/

		public async Task<List<UnifiedRequestDto>> GetNewApplicationsForWidgetAsync()
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
					Detail = $"Applied for: {j.JobListing!.ExternalTitle} ({j.SuitabilityScore}%)",
					Date = j.AppliedAt,
					Status = LeaveStatus.Pending
				})
				.ToListAsync();
		}

		public async Task<List<JobApplication>> GetApplicationsByStatusAsync(
			ApplicationStatus status)
		{
			using var db = await _dbFactory.CreateDbContextAsync();

			return await db.JobApplications
				.Include(j => j.JobListing)
				.Include(j => j.Country)
				.Include(j => j.County)
				.Include(j => j.Education)
				.Include(j => j.Experience)
				.Where(j => j.Status == status)
				.OrderByDescending(j => j.AppliedAt)
				.ToListAsync();
		}

		public async Task<bool> UpdateApplicationStatusAsync(
			Guid id,
			ApplicationStatus status)
		{
			using var db = await _dbFactory.CreateDbContextAsync();

			var app = await db.JobApplications.FindAsync(id);
			if (app == null) return false;

			app.Status = status;
			return await db.SaveChangesAsync() > 0;
		}
		public async Task<JobApplication?> GetByIdAsync(Guid id)
		{
			using var db = await _dbFactory.CreateDbContextAsync();

			return await db.JobApplications
				.Include(a => a.JobListing)
				.Include(a => a.Experience)
				.Include(a => a.Education)
				.Include(a => a.Country)
				.Include(a => a.County)
				.FirstOrDefaultAsync(a => a.Id == id);
		}

	}
}
