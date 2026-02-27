using HR.Services.Interfaces;
using System;
using System.Threading.Tasks;
using HR.Services.DTO;

namespace HR.Web.Public.Blazor.Components.Services
{
	public class PublicGuestService : ICurrentUserService
	{
		// No one is logged in on the public site
		public Task<Guid> GetCurrentUserIdAsync() => Task.FromResult(Guid.Empty);

		// Returning null because guests aren't employees
		public Task<CurrentEmployeeDetailsDto?> GetCurrentEmployeeDetailsAsync()
			=> Task.FromResult<CurrentEmployeeDetailsDto?>(null);
	}
}
