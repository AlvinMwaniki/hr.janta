using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Data.Models.Employees;

public class DisabilityDetail
{
	[Key]
	public Guid Id { get; set; }

	// Relationship to Employee
	public Guid EmployeeId { get; set; }
	[ForeignKey("EmployeeId")]
	public Employee Employee { get; set; } = default!;

	[Required(ErrorMessage = "Please specify the nature of the disability")]
	public string DisabilityNature { get; set; } = string.Empty;

	[Required(ErrorMessage = "NCPWD Number is required")]
	public string NCPWD_Number { get; set; } = string.Empty;

	public string? KRA_ExemptionNumber { get; set; }

	// File Storage: We store the PATH, not the actual file in the DB
	public string? CertificateFileName { get; set; }
	public string? CertificateFilePath { get; set; }

	public DateTime? ExpiryDate { get; set; }
	public DateTime CreatedAt { get; set; } = DateTime.Now;
}