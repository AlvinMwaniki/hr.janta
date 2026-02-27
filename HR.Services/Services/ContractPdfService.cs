using HR.Services.DTO;

using Microsoft.AspNetCore.Hosting;

using System.Diagnostics;

namespace HR.Services.Services
{
	public class ContractPdfService
	{
		private readonly IWebHostEnvironment _env;

		public ContractPdfService(IWebHostEnvironment env)
		{
			_env = env;
		}

		// Generate HTML for preview modal
		public string GenerateHtml(EmploymentContractModel model)
		{
			var templatePath = Path.Combine(_env.WebRootPath, "templates", "contract-modern.html");
			var html = File.ReadAllText(templatePath);

			html = html
				.Replace("{{EmployeeName}}", model.EmployeeName)
				.Replace("{{JobTitle}}", model.JobTitle)
				.Replace("{{DepartmentName}}", model.DepartmentName)
				.Replace("{{StartDate}}", model.StartDate.ToString("MMMM dd, yyyy"))
				.Replace("{{Email}}", model.Email)
				.Replace("{{Phone}}", model.Phone)
				.Replace("{{Estate}}", model.Estate)
				.Replace("{{SubCounty}}", model.SubCounty)
				.Replace("{{County}}", model.County)
				.Replace("{{GeneratedDate}}", DateTime.Now.ToString("MMMM dd, yyyy"));

			return html;
		}

		// Generate PDF using wkhtmltopdf.exe directly
		public byte[] Generate(EmploymentContractModel model)
		{
			var html = GenerateHtml(model);

			// 1. Save HTML to temp file
			var tempHtml = Path.GetTempFileName() + ".html";
			File.WriteAllText(tempHtml, html);

			// 2. Output PDF temp file
			var tempPdf = Path.GetTempFileName() + ".pdf";

			// 3. Path to wkhtmltopdf.exe inside your project
			var exePath = Path.Combine(_env.ContentRootPath, "DinkToPdfLibs", "wkhtmltopdf.exe");

			var psi = new ProcessStartInfo
			{
				FileName = exePath,
				Arguments = $"\"{tempHtml}\" \"{tempPdf}\"",
				UseShellExecute = false,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				CreateNoWindow = true
			};

			var process = Process.Start(psi);
			if (process is null)
			{
				throw new InvalidOperationException($"Failed to start process: {exePath}");
			}
			process.WaitForExit();

			// 4. Read PDF bytes
			var pdfBytes = File.ReadAllBytes(tempPdf);

			// 5. Cleanup
			File.Delete(tempHtml);
			File.Delete(tempPdf);

			return pdfBytes;
		}
	}
}