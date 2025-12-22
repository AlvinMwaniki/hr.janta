// HR.Services/Interfaces/IUserClaimService.cs

using System.Security.Claims;

namespace HR.Services.Interfaces
{
	public interface IUserClaimService
	{
		Task<IEnumerable<Claim>> GetCustomClaimsForUserAsync(Guid userId);
	}
}