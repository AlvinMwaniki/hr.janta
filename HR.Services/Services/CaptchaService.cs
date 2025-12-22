// HR.Services/Services/CaptchaService.cs

using HR.Utilities;

using System.Threading.Tasks;

namespace HR.Services.Services;
public class CaptchaService
{
	// Keep the default parameterless constructor or register other required services

	public Task<CaptchaResult> GenerateCaptchaAsync()
	{
		string code = Captcha.GenerateCaptchaCode();
		// Uses the image generation logic
		var result = Captcha.GenerateCaptchaImage(180, 50, code);

		// result.CaptchaCode is already correctly set inside GenerateCaptchaImage, perfect.

		return Task.FromResult(result);
	}

	// ⭐ CRITICAL FIX: Replace the failing async method with a simple sync method ⭐
	// This new method takes the expected code (from the cookie) and the user's input.
	public bool ValidateCaptcha(string expectedCode, string userInput)
	{
		if (string.IsNullOrEmpty(expectedCode))
		{
			// If the cookie wasn't found, validation fails.
			return false;
		}

		// Use the correct comparison
		return expectedCode.Equals(userInput, StringComparison.OrdinalIgnoreCase);
	}

	// ⭐ DELETE the entire obsolete ValidateCaptchaAsync method that used ProtectedSessionStorage ⭐
}