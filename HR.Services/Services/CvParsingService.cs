using HR.Services.DTO;
using HR.Services.Interfaces;

using Microsoft.Extensions.Logging;

using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

using System.Text.Json;
using System.Text.RegularExpressions;


namespace HR.Services.Services
{
	public class CvParsingService : ICvParsingService
	{
		private readonly ILogger<CvParsingService> _logger;
		private readonly OllamaClient _ollama;

		public CvParsingService(ILogger<CvParsingService> logger, OllamaClient ollama)
		{
			_logger = logger;
			_ollama = ollama;
		}

		// =====================================================
		// MAIN ENTRY
		// =====================================================
		public async Task<CvAnalysisResult> ParseAsync(
			byte[] pdfBytes,
			string jobDescription,
			string jobTitle)
		{
			// Extract text from PDF
			var words = ExtractWords(pdfBytes);
			var text = string.Join("\n", words.Select(w => w.Text));

			// Build AI prompt
			string prompt = BuildCvPrompt(text);

			// Call Ollama
			string json = await _ollama.GenerateAsync("llama3.2", prompt);

			CvAnalysisResult? result = null;

			try
			{
				result = JsonSerializer.Deserialize<CvAnalysisResult>(json,
					new JsonSerializerOptions
					{
						PropertyNameCaseInsensitive = true
					});
			}
			catch
			{
				// Fallback: extract JSON inside text
				var match = Regex.Match(json, "{.*}", RegexOptions.Singleline);
				if (match.Success)
				{
					result = JsonSerializer.Deserialize<CvAnalysisResult>(match.Value);
				}
			}

			if (result == null)
				throw new Exception("AI returned invalid JSON.");

			EnhanceResult(result, jobDescription);

			return result;
		}

		// =====================================================
		// PROMPT BUILDER
		// =====================================================
		private string BuildCvPrompt(string cvText)
		{
			return $@"
You are an expert CV parser. Extract structured JSON ONLY.
Do NOT add explanations, comments, markdown, or text outside JSON.

Return JSON EXACTLY in this format:

{{
  ""full_name"": """",
  ""email"": """",
  ""phoneNumber"": """",
  ""education"": [
    {{
      ""institution"": """",
      ""field"": """",
      ""level"": """",
      ""startDate"": null,
      ""endDate"": null
    }}
  ],
  ""experience"": [
    {{
      ""company"": """",
      ""jobTitle"": """",
      ""responsibilities"": """",
      ""startDate"": null,
      ""endDate"": null
    }}
  ],
  ""skills_found"": []
}}

Rules:
- Extract the candidate's REAL name (not company names).
- Extract ONLY the candidate's email (ignore HR, support, info@, ops@).
- Extract ALL education entries.
- Extract ALL experience entries.
- Convert dates to YYYY-MM-DD format when possible.
- If a date is missing, return null.
- Responsibilities should be a single string with bullet points separated by '\n'.
- Skills_found must be a list of strings.

CV TEXT:
{cvText}
";
		}

		// =====================================================
		// PDF TEXT EXTRACTION
		// =====================================================
		private List<Word> ExtractWords(byte[] pdfBytes)
		{
			using var ms = new MemoryStream(pdfBytes);
			var words = new List<Word>();

			using var document = PdfDocument.Open(ms);

			foreach (var page in document.GetPages())
				words.AddRange(page.GetWords());

			return words;
		}

		// =====================================================
		// SCORING
		// =====================================================
		private void EnhanceResult(CvAnalysisResult result, string jobDescription)
		{
			int score = 0;

			if (!string.IsNullOrWhiteSpace(result.FullName)) score += 10;
			if (!string.IsNullOrWhiteSpace(result.Email)) score += 10;
			if (!string.IsNullOrWhiteSpace(result.PhoneNumber)) score += 10;

			score += Math.Min(result.Education.Count * 5, 20);
			score += Math.Min(result.Experience.Count * 6, 30);
			score += Math.Min(result.SkillsFound.Count * 4, 20);

			// Bonus: match job description keywords
			var jdWords = jobDescription.ToLower().Split(' ');
			var matches = result.SkillsFound.Count(s => jdWords.Contains(s.ToLower()));
			score += matches * 2;

			result.SuitabilityScore = Math.Min(score, 100);
			result.IsRejected = result.SuitabilityScore < 50;

			result.Analysis =
				$"Score: {result.SuitabilityScore}/100 | " +
				(result.IsRejected ? "Below threshold" : "Strong candidate");
		}
	}
}