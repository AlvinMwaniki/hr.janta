using HR.Data;
using HR.Data.Models.Recruitment;
using HR.Services.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace HR.Services.Services
{
	public class InterviewService : IInterviewService
	{
		private readonly IDbContextFactory<HRDbContext> _dbFactory;

		public InterviewService(IDbContextFactory<HRDbContext> dbFactory)
		{
			_dbFactory = dbFactory;
		}

		public async Task<Interview?> GetByApplicationIdAsync(Guid appId)
		{
			using var db = await _dbFactory.CreateDbContextAsync();
			return await db.Interviews
				.Include(i => i.JobApplication)
				.FirstOrDefaultAsync(i => i.JobApplicationId == appId);
		}

		public async Task<Interview> CreateOrUpdateAsync(Guid appId, DateTime date, string? time, string? notes)
		{
			using var db = await _dbFactory.CreateDbContextAsync();

			var interview = await db.Interviews
				.FirstOrDefaultAsync(i => i.JobApplicationId == appId);

			if (interview == null)
			{
				interview = new Interview
				{
					Id = Guid.NewGuid(),
					JobApplicationId = appId,
					InterviewDate = date,
					InterviewTime = time,
					Notes = notes,
					Outcome = InterviewOutcome.Pending
				};
				db.Interviews.Add(interview);
			}
			else
			{
				interview.InterviewDate = date;
				interview.InterviewTime = time;
				interview.Notes = notes;
				interview.UpdatedAt = DateTime.UtcNow;
			}

			await db.SaveChangesAsync();
			return interview;
		}

		public async Task<bool> SetOutcomeAsync(Guid appId, InterviewOutcome outcome)
		{
			using var db = await _dbFactory.CreateDbContextAsync();

			var interview = await db.Interviews
				.FirstOrDefaultAsync(i => i.JobApplicationId == appId);

			if (interview == null) return false;

			interview.Outcome = outcome;
			interview.UpdatedAt = DateTime.UtcNow;

			var app = await db.JobApplications.FirstOrDefaultAsync(a => a.Id == appId);
			if (app != null)
			{
				if (outcome == InterviewOutcome.Passed)
					app.Status = ApplicationStatus.Hired;      // or ApplicationStatus.Interviewed if you want another step
				else if (outcome == InterviewOutcome.Failed)
					app.Status = ApplicationStatus.Rejected;
			}

			return await db.SaveChangesAsync() > 0;
		}
	}
}