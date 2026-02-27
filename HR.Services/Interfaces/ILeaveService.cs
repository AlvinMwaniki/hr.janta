// HR.Services/Interfaces/ILeaveService.cs

using HR.Core.Enums;
using HR.Services.DTO;

using System.Threading.Tasks;

public interface ILeaveService
{
	Task<bool> SubmitLeaveRequestAsync(LeaveSubmissionDto dto);
	Task<List<LeaveRequestViewDto>> GetPendingRequestsAsync();
	Task<bool> ReviewLeaveRequestAsync(Guid leaveId, LeaveStatus newStatus, string? comment = null);
	Task<bool> CancelLeaveRequestAsync(Guid leaveId);
	Task<List<LeaveRequestViewDto>> GetMyLeaveRequestsAsync();
	Task<List<LeaveRequestViewDto>> GetAllRequestsAsync();
}