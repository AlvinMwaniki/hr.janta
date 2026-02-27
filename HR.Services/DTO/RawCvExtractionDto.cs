using HR.Services.DTO;

public class RawCvExtractionDto
{
	public string? full_name { get; set; }
	public string? email { get; set; }
	public string? PhoneNumber { get; set; }
	public List<ExperienceDto>? experience { get; set; }
	public List<EducationDto>? education { get; set; }
	public List<string>? skills_found { get; set; }
}
