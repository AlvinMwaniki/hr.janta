// HR.Services/Interfaces/ILeaveService.cs

using HR.Services.DTO;

using System.Threading.Tasks;

public interface ILeaveService
{
	Task<bool> SubmitLeaveRequestAsync(LeaveSubmissionDto dto);
}