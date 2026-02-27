using HR.Data.Models.Recruitment;

using HR.Services.DTO.Recruitment;
using HR.Services.DTO;
namespace HR.Services.Interfaces
{
	public interface IRequisitionService
	{
		Task<bool> CreateAsync(RequisitionCreateDto dto);
		Task<bool> SubmitAsync(Guid requisitionId);
		Task<List<RequisitionViewDto>> GetAllAsync();
		Task<List<RequisitionViewDto>> GetMyRequisitionsAsync();
		Task<bool> ApproveAsync(Guid requisitionId);
		Task<bool> RejectAsync(Guid requisitionId, string comment);
		Task<List<DepartmentDto>> GetDepartmentsAsync();
		Task<RequisitionViewDto?> GetByIdAsync(Guid id);
		Task<bool> DeleteAsync(Guid requisitionId);
		Task<bool> PublishJobAsync(Guid requisitionId, string externalTitle, string location);
		Task<List<JobListing>> GetJobListingsAsync();
		// For the Public Site
		Task<PublicJobDto?> GetPublicJobByIdAsync(Guid listingId);

		// For the Admin Site to manage listings
		Task<bool> ToggleListingStatusAsync(Guid listingId);
	}
}
