using System.Threading.Tasks;

using HR.Core.DTOs;

namespace HR.Core.Interfaces
{
	public interface IEmailSender
	{
		Task SendEmailAsync(EmailMessage emailMessage);
	}
}
