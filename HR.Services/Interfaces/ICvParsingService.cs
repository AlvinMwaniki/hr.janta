using Microsoft.AspNetCore.Components.Forms;
using HR.Services.DTO;

namespace HR.Services.Interfaces
{
	public interface ICvParsingService
	{
		Task<CvAnalysisResult> ParseAsync(byte[] pdfBytes, string jobDescription, string jobTitle);
	}
}
