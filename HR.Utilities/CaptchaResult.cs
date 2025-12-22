
using System;


namespace HR.Utilities
{
	public class CaptchaResult
	{
		// The plain text code generated (e.g., "A5B9")
		public string CaptchaCode { get; set; } = string.Empty;

		// The raw image data (PNG format)
		public byte[] CaptchaByteData { get; set; } = Array.Empty<byte>();

		// The image data encoded as Base64 for direct use in Blazor/HTML <img> tags
		public string CaptchBase64Data { get; set; } = string.Empty;

		public DateTime Timestamp { get; set; }
	}
}