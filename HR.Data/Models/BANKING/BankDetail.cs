using System;

using HR.Data.Models.Employees;

namespace HR.Data.Models.BANKING;

public class BankDetail
{
	public Guid Id { get; set; }
	// Link to PaymentData 1:1
	//public Guid PaymentDataId { get; set; }
	//public PaymentData PaymentData { get; set; } = default!;

	// Foreign key reference to Employee
	public Guid EmployeeId { get; set; }
	public Employee Employee { get; set; } = default!;

	// Bank Information
	public string BankName { get; set; } = default!;
	public string AccountName { get; set; } = default!;
	public string AccountNumber { get; set; } = default!;
	public string Branch { get; set; } = default!;

	// --- NEW NAVIGATION PROPERTY BACK TO DEPENDENT PaymentData ---
	public PaymentData? PaymentData { get; set; }
	// -----------------------------------------------------------
}
