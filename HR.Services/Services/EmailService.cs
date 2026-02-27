// HR.Services/Services/EmailService.cs
using HR.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace HR.Services.Services
{
	public class EmailService : IEmailService
	{


		public Task SendSetupPasswordEmailAsync(string recipientEmail, string recipientName, string setupLink)
		{
			// --- MOCK IMPLEMENTATION ---
			//  (SendGrid, SMTP, etc.)

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
		public Task SendEmailAsync(string toEmail, string subject, string body)
		{
			// --- MOCK IMPLEMENTATION ---
			//  (SendGrid, SMTP, etc.)
			Console.WriteLine("=============================================");
			Console.WriteLine($"EMAIL SIMULATION");
			Console.WriteLine($"TO: {toEmail}");
			Console.WriteLine($"SUBJECT: {subject}");
			Console.WriteLine($"---------------------------------------------");
			Console.WriteLine(body);
			Console.WriteLine("=============================================");
			return Task.CompletedTask;
		}
		public Task SendInterviewInviteAsync(string toEmail, string candidateName, string jobTitle, DateTime date, string? time)
		{
			Console.WriteLine("=============================================");
			Console.WriteLine($"EMAIL SIMULATION");
			Console.WriteLine($"TO: {toEmail}");
			Console.WriteLine($"SUBJECT: {candidateName}");
			Console.WriteLine($"---------------------------------------------");
			Console.WriteLine(jobTitle);
			Console.WriteLine("=============================================");
			return Task.CompletedTask;
		}
	}
}