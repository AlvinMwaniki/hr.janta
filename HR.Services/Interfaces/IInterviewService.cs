using HR.Data.Models.Recruitment;

namespace HR.Services.Interfaces
{
	public interface IInterviewService
	{
		Task<Interview?> GetByApplicationIdAsync(Guid appId);
		Task<Interview> CreateOrUpdateAsync(Guid appId, DateTime date, string? time, string? notes);
		Task<bool> SetOutcomeAsync(Guid appId, InterviewOutcome outcome);
	}
}