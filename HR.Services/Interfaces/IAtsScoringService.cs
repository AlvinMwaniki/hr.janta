using HR.Data.Models.Recruitment;

namespace HR.Services.Interfaces
{
	public interface IAtsScoringService
	{
		Task<(int Score, string Reason, List<string> MatchedSkills)>
			ScoreAsync(JobApplication application, JobListing listing);
	}
}