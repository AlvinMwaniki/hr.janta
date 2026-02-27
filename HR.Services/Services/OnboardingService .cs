using HR.Data;
using HR.Data.Models.Recruitment;
using HR.Services.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace HR.Services.Services
{
	public class OnboardingService : IOnboardingService
	{
		private readonly IDbContextFactory<HRDbContext> _dbFactory;

		public OnboardingService(IDbContextFactory<HRDbContext> dbFactory)
		{
			_dbFactory = dbFactory;
		}

		public async Task<Onboarding?> GetByApplicationIdAsync(Guid appId)
		{
			using var db = await _dbFactory.CreateDbContextAsync();
			return await db.Onboardings
				.Include(o => o.JobApplication)
				.FirstOrDefaultAsync(o => o.JobApplicationId == appId);
		}

		public async Task<Onboarding> CreateOrUpdateAsync(Onboarding onboarding)
		{
			using var db = await _dbFactory.CreateDbContextAsync();

			var existing = await db.Onboardings
				.FirstOrDefaultAsync(o => o.JobApplicationId == onboarding.JobApplicationId);

			if (existing == null)
			{
				onboarding.Id = Guid.NewGuid();
				db.Onboardings.Add(onboarding);
			}
			else
			{
				existing.KRAProvided = onboarding.KRAProvided;
				existing.NSSFProvided = onboarding.NSSFProvided;
				existing.NHIFProvided = onboarding.NHIFProvided;
				existing.BankDetailsProvided = onboarding.BankDetailsProvided;
				existing.BackgroundCheckPassed = onboarding.BackgroundCheckPassed;
				existing.UpdatedAt = DateTime.UtcNow;
			}

			await db.SaveChangesAsync();
			return onboarding;
		}

		public async Task<bool> MarkAsHiredAsync(Guid appId)
		{
			using var db = await _dbFactory.CreateDbContextAsync();

			var app = await db.JobApplications.FindAsync(appId);
			if (app == null) return false;

			app.Status = ApplicationStatus.Hired;
			return await db.SaveChangesAsync() > 0;
		}

		public async Task<bool> MarkAsRejectedAsync(Guid appId)
		{
			using var db = await _dbFactory.CreateDbContextAsync();

			var app = await db.JobApplications.FindAsync(appId);
			if (app == null) return false;

			app.Status = ApplicationStatus.Rejected;
			return await db.SaveChangesAsync() > 0;
		}
	}
}