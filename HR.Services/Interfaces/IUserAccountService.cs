using HR.Services.DTO;

using System;
using System.Threading.Tasks;

namespace HR.Services.Interfaces;

public interface IUserAccountService
{
	Task<Guid> CreateUserAsync(UserCreationDto dto);

	Task<UserEditDto?> GetUserForEditAsync(Guid userId);

	Task UpdateUserGeneralInfoAsync(UserEditDto dto);

	Task ChangePasswordAsync(UserPasswordChangeDto dto);
}