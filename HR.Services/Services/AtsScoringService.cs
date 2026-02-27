using HR.Data.Models.Recruitment;
using HR.Services.Interfaces;

namespace HR.Services.Services
{
	public class AtsScoringService : IAtsScoringService
	{
		private double CalculateTotalExperience(JobApplication app)
		{
			double total = 0;

			foreach (var exp in app.Experience)
			{
				var end = exp.EndDate ?? DateTime.UtcNow;
				var years = (end - exp.StartDate).TotalDays / 365.0;
				total += years;
			}

			return Math.Round(total, 1);
		}
		public Task<(int Score, string Reason, List<string> MatchedSkills)>
			ScoreAsync(JobApplication application, JobListing listing)
		{
			int score = 0;
			var reasons = new List<string>();
			var matchedSkills = new List<string>();

			var jobText = (listing.ExternalDescription ?? "").ToLower();

			// ---------------------------------------------------------
			// 1️⃣ SKILLS MATCHING (40 POINTS)
			// ---------------------------------------------------------
			var applicantSkills = (application.SkillsFound ?? "")
				.Split(',', StringSplitOptions.RemoveEmptyEntries)
				.Select(s => s.Trim().ToLower())
				.ToList();

			var requiredSkills = (listing.RequiredSkills ?? "")
				.Split(',', StringSplitOptions.RemoveEmptyEntries)
				.Select(s => s.Trim().ToLower())
				.ToList();

			foreach (var skill in applicantSkills)
			{
				if (requiredSkills.Contains(skill) || jobText.Contains(skill))
					matchedSkills.Add(skill);
			}

			int skillPoints = Math.Min(matchedSkills.Count * 10, 40);
			score += skillPoints;

			if (matchedSkills.Any())
				reasons.Add($"Matched skills: {string.Join(", ", matchedSkills)}");

			// ---------------------------------------------------------
			// 2️⃣ EXPERIENCE MATCHING (30 POINTS)
			// ---------------------------------------------------------
			double totalYears = CalculateTotalExperience(application);

			if (totalYears > 0)
			{
				int expPoints = Math.Min((int)(totalYears * 3), 30);
				score += expPoints;

				reasons.Add($"Total experience: {totalYears} years");
			}


			// ---------------------------------------------------------
			// 3️⃣ EDUCATION MATCHING (20 POINTS)
			// ---------------------------------------------------------
			if (application.Education.Any())
			{
				int eduPoints = Math.Min(application.Education.Count * 5, 20);
				score += eduPoints;

				reasons.Add($"Education entries: {application.Education.Count}");
			}

			// ---------------------------------------------------------
			// 4️⃣ LOCATION MATCHING (10 POINTS)
			// ---------------------------------------------------------
			if (!string.IsNullOrWhiteSpace(listing.Location))
			{
				if (application.CountyId != null &&
					listing.Location.Contains(application.County?.Name ?? "", StringComparison.OrdinalIgnoreCase))
				{
					score += 10;
					reasons.Add("Location matches job posting");
				}
			}

			// ---------------------------------------------------------
			// FINALIZE SCORE
			// ---------------------------------------------------------
			score = Math.Min(score, 100);

			if (!reasons.Any())
				reasons.Add("No strong matches found.");

			return Task.FromResult((score, string.Join("; ", reasons), matchedSkills));
		}
	}
}