// HR.Utilities/Captcha.cs

using System;
using System.IO;
using System.Linq;
using System.Text;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;        // Provides Mutate() and Clear()
using SixLabors.ImageSharp.Drawing.Processing; // Provides DrawLine() and DrawText()
using SixLabors.Fonts;
using SixLabors.ImageSharp.Drawing;
using System.Drawing; // Still needed for PointF/RectangleF in some environments

namespace HR.Utilities
{
	public static class Captcha
	{
		public const string Letters = "23456789ABCDEFGHJKLMNPRSTUVWXYZ";
		public static readonly Random Rand = new Random();

		public static readonly Font DefaultBaseFont;

		static Captcha()
		{
			var fontCollection = new FontCollection();
			fontCollection.AddSystemFonts();

			// Check if any fonts were loaded at all
			if (!fontCollection.Families.Any())
			{
				throw new InvalidOperationException("No suitable font could be loaded for CAPTCHA. The application could not find any system fonts.");
			}

			// Get the first available font, guaranteed to be non-null
			FontFamily fontFamily = fontCollection.Families.First();

			// Create the base font object
			DefaultBaseFont = fontFamily.CreateFont(1f, FontStyle.Bold);
		}

		public static string GenerateCaptchaCode()
		{
			int maxRand = Letters.Length - 1;
			StringBuilder sb = new StringBuilder();

			for (int i = 0; i < 4; i++)
			{
				int index = Rand.Next(maxRand);
				sb.Append(Letters[index]);
			}
			return sb.ToString();
		}

		public static CaptchaResult GenerateCaptchaImage(int width, int height, string captchaCode)
		{
			// Calculate font size dynamically
			var fontSize = (int)((width / (float)captchaCode.Length) * 1.2);

			// Use the base font and scale it to the correct size
			var captchaFont = new Font(DefaultBaseFont, fontSize);

			// TextOptions is still needed to set alignment
			var textOptions = new TextOptions(captchaFont)
			{
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center
			};

			using (var image = new Image<Rgba32>(width, height))
			{
				// Clear background
				image.Mutate(x => x.Clear(SixLabors.ImageSharp.Color.Parse("#F0F0F0")));

				// Add distortion lines (using fully qualified names for PointF/Color)
				for (int i = 0; i < 5; i++)
				{
					image.Mutate(x => x.DrawLine(
						SixLabors.ImageSharp.Color.DarkGray, 2f,
						new SixLabors.ImageSharp.PointF(Rand.Next(width), Rand.Next(height)),
						new SixLabors.ImageSharp.PointF(Rand.Next(width), Rand.Next(height))));
				}

				// Draw Captcha Code (using the final, working signature: Text, Font, Color, Location)
				var textLocation = new SixLabors.ImageSharp.PointF(0, 0);

				image.Mutate(x => x.DrawText(
					// 1. The Text (string)
					captchaCode,
					// 2. The Font Object
					captchaFont,
					// 3. The Color/Brush
					SixLabors.ImageSharp.Color.DarkBlue,
					// 4. The Location (PointF)
					textLocation)
				);

				using (var ms = new MemoryStream())
				{
					image.Save(ms, PngFormat.Instance);
					// CaptchaBase64Data property is set here
					return new CaptchaResult { CaptchaCode = captchaCode, CaptchaByteData = ms.ToArray(), CaptchBase64Data = Convert.ToBase64String(ms.ToArray()), Timestamp = DateTime.Now };
				}
			}
		}
	}
}