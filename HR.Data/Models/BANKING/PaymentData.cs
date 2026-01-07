using HR.Data.Models.Employees;

using System;
using System.ComponentModel.DataAnnotations;

namespace HR.Data.Models.BANKING;

public class PaymentData
{
	public Guid Id { get; set; }

	// One-to-One relation with Employee
	public Guid EmployeeId { get; set; }
	public Employee Employee { get; set; } = default!;

	// Government Identifiers
	[Required(ErrorMessage = "KRA PIN is required")]
	[StringLength(11, MinimumLength = 11, ErrorMessage = "KRA PIN must be exactly 11 characters.")]
	[RegularExpression(@"^[A|a]\d{9}[A-Za-z]$",
		ErrorMessage = "KRA PIN must start with 'A', followed by 9 digits, and end with a letter (e.g., A123456789B).")]
	public string? KRA_PIN { get; set; }

	public string? NSSF_Number { get; set; }
	public string? NHIF_Number { get; set; } // SHA

	// --- NEW FOREIGN KEY LINK TO BankDetail ---
	public Guid BankDetailId { get; set; } // The ID of the related BankDetail
	public BankDetail BankDetail { get; set; } = default!; // The required navigation property
														   // ------------------------------------------

	// Bank Details
	

}
