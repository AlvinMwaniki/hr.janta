using HR.Data.Models.Recruitment;

namespace HR.Services.Interfaces
{
	public interface IOnboardingService
	{
		Task<Onboarding?> GetByApplicationIdAsync(Guid appId);
		Task<Onboarding> CreateOrUpdateAsync(Onboarding onboarding);
		Task<bool> MarkAsHiredAsync(Guid appId);
		Task<bool> MarkAsRejectedAsync(Guid appId);
	}
}