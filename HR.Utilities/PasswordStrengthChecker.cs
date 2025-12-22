// HR.Utilities/PasswordStrengthChecker.cs

using System.Text.RegularExpressions;

namespace HR.Utilities
{
	public enum PasswordStrength
	{
		Invalid = 0,
		Weak = 1,
		Moderate = 2,
		Strong = 3
	}

	public static class PasswordStrengthChecker
	{
		public static PasswordStrength Check(string password)
		{
			if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
			{
				return PasswordStrength.Invalid;
			}

			int score = 0;

			// Rule 1: Length (already checked, but adds base score)
			if (password.Length >= 10) score++;

			// Rule 2: Lowercase letters
			if (Regex.IsMatch(password, @"[a-z]")) score++;

			// Rule 3: Uppercase letters
			if (Regex.IsMatch(password, @"[A-Z]")) score++;

			// Rule 4: Numbers
			if (Regex.IsMatch(password, @"\d")) score++;

			// Rule 5: Special characters
			if (Regex.IsMatch(password, @"[^\da-zA-Z]")) score++;

			// Map score to strength
			if (score <= 1) return PasswordStrength.Weak;
			if (score <= 3) return PasswordStrength.Moderate;
			return PasswordStrength.Strong;
		}
	}
}