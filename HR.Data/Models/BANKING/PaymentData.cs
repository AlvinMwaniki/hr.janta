using System;

using HR.Data.Models.Employees;

namespace HR.Data.Models.BANKING;

public class PaymentData
{
	public Guid Id { get; set; }

	// One-to-One relation with Employee
	public Guid EmployeeId { get; set; }
	public Employee Employee { get; set; } = default!;

	// Government Identifiers
	public string? KRA_PIN { get; set; }
	public string? NSSF_Number { get; set; }
	public string? NHIF_Number { get; set; } // SHA

	// --- NEW FOREIGN KEY LINK TO BankDetail ---
	public Guid BankDetailId { get; set; } // The ID of the related BankDetail
	public BankDetail BankDetail { get; set; } = default!; // The required navigation property
														   // ------------------------------------------

	// Bank Details
	

}
