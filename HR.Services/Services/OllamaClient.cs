using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Json;


namespace HR.Services.Services
{
	public class OllamaClient
	{
		private readonly HttpClient _http;

		public OllamaClient(HttpClient http)
		{
			_http = http;
		}

		public async Task<string> GenerateAsync(string model, string prompt)
		{
			var request = new
			{
				model = model,
				prompt = prompt,
				stream = false
			};

			var response = await _http.PostAsJsonAsync("/api/generate", request);
			response.EnsureSuccessStatusCode();

			var result = await response.Content.ReadFromJsonAsync<OllamaGenerateResponse>();

			if (result == null || string.IsNullOrWhiteSpace(result.response))
				throw new Exception("Ollama returned an empty or invalid response.");

			return result.response;
		}

		private class OllamaGenerateResponse
		{
			public string? response { get; set; }
		}
	}

}
