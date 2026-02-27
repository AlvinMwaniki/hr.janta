using HR.Data.Models.Recruitment;
using HR.Services.DTO;

using Microsoft.AspNetCore.Components.Forms;

namespace HR.Services.Interfaces
{
	public interface IJobApplicationService
	{
		Task<bool> SubmitApplicationAsync(JobApplication application, IBrowserFile file);
		Task<List<UnifiedRequestDto>> GetNewApplicationsForWidgetAsync();
		Task<List<JobApplication>> GetApplicationsByStatusAsync(ApplicationStatus status);
		Task<bool> UpdateApplicationStatusAsync(Guid id, ApplicationStatus status);
		Task<JobApplication?> GetByIdAsync(Guid id);
	}
}