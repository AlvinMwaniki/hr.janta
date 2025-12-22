// HR.Services/Services/IEmailService.cs
using System.Threading.Tasks;

namespace HR.Services.Services
{
	public interface IEmailService
	{
		Task SendSetupPasswordEmailAsync(string recipientEmail, string recipientName, string setupLink);
	}
}