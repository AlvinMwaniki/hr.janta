// HR.Services/Services/EmailService.cs

namespace HR.Services.Services
{
	public class EmailService : IEmailService
	{
		public Task SendSetupPasswordEmailAsync(string recipientEmail, string recipientName, string setupLink)
		{
			// --- MOCK IMPLEMENTATION ---
			// In production, this is where you'd use a real client (SendGrid, SMTP, etc.)

			Console.WriteLine("=============================================");
			Console.WriteLine($"EMAIL SIMULATION: Employee Setup Initiated");
			Console.WriteLine($"TO: {recipientName} <{recipientEmail}>");
			Console.WriteLine($"SUBJECT: Set Up Your HR Portal Password");
			Console.WriteLine($"---------------------------------------------");
			Console.WriteLine($"Please use this secure link to set your password:");
			Console.WriteLine($"LINK: {setupLink}");
			Console.WriteLine("=============================================");

			return Task.CompletedTask;
		}
	}
}