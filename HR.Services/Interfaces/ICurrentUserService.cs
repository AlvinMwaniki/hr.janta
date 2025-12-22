// HR.Services/Interfaces/ICurrentUserService.cs

using HR.Services.DTO;

using System;
using System.Threading.Tasks;

public interface ICurrentUserService
{
	Task<Guid> GetCurrentUserIdAsync();

	Task<CurrentEmployeeDetailsDto?> GetCurrentEmployeeDetailsAsync();
}