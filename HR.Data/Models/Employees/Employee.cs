using HR.Data.Models.Advances;
using HR.Data.Models.BANKING;
using HR.Data.Models.Departments;
using HR.Data.Models.Leaves;
using HR.Data.Models.County;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HR.Data.Models.Employees
{
	public class Employee
	{
		// CHAPI
		public byte[]? Photo { get; set; }

		// Helper to display the image in the UI
		public string PhotoBase64
		{
			get
			{
				if (Photo != null && Photo.Length > 0)
				{
					return $"data:image/png;base64,{Convert.ToBase64String(Photo)}";
				}
				// Fallback to the physical file in wwwroot/
				return "lib\\Images\\user.png";
			}
		}


		public Guid Id { get; set; }
		[Required(ErrorMessage = "Employee Code is required")]
		public string EmployeeCode { get; set; } = default!;

		// Basic Info
		[Required(ErrorMessage = "First Name is required")]
		public string FirstName { get; set; } = default!;
		public string? MiddleName { get; set; }

		[Required(ErrorMessage = "Last Name is required")]
		public string LastName { get; set; } = default!;

		[Required(ErrorMessage = "Email is required")]
		[EmailAddress(ErrorMessage = "Invalid email address")]
		[RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Please enter a valid email address")]
		public string Email { get; set; } = default!;

		// Inside Employee.cs

		[Required(ErrorMessage = "Country code is required")]
		public Guid? CountryId { get; set; }
		// Navigation property (Optional but good for reporting)
		public HR.Data.Models.Country.Country? Country { get; set; }

		public Guid? CountyId { get; set; }
		public HR.Data.Models.County.County? County { get; set; }
		// Location Row 2
		// Link to SubCounty
		public Guid? SubCountyId { get; set; }
		public SubCounty? SubCounty { get; set; }

		public string? Estate { get; set; }      // e.g., Clay City, Nyayo Estate, Milimani
		public string? POBox { get; set; }       // e.g., 00100-54321

		[Required(ErrorMessage = "Phone number is required")]
		[StringLength(10, MinimumLength = 9, ErrorMessage = "Phone number is invalid. Should be 9 or 10 digits.")]
		[RegularExpression(@"^[0-9]*$", ErrorMessage = "Only numbers are allowed.")] 
		public string Phone { get; set; } = default!;

		/*[Required(ErrorMessage = "Address is required")]
		public string Address { get; set; } = default!;*/

		[Required(ErrorMessage = "Date of Birth is required")]
		public DateTime DOB { get; set; }

		[Required(ErrorMessage = "National ID is required")]
		[StringLength(10, MinimumLength = 10, ErrorMessage = "National ID must be exactly 10 digits.")]
		[RegularExpression(@"^\d{10}$", ErrorMessage = "National ID must contain only numbers.")]
		public string? NationalID { get; set; }

		[Required(ErrorMessage = "Gender is required")]
		public string? Gender { get; set; }

		[Required(ErrorMessage = "Job Title is required")]
		public string? JobTitle { get; set; } // NEW

		[Required(ErrorMessage = "Disability is required")]
		public string? Disability { get; set; }
		public virtual DisabilityDetail? DisabilityDetail { get; set; } = new();
		// Ethnicity is optional
		public Guid? EthnicityId { get; set; }
		public Ethnicity? Ethnicity { get; set; }

		// Department FK
		[Required(ErrorMessage = "Department is required")]
		public Guid DepartmentId { get; set; }
		public Department Department { get; set; } = default!;

		[Required(ErrorMessage = "Hire Date is required")]
		public DateTime HireDate { get; set; }

		[Required(ErrorMessage = "Status is required")]
		public string Status { get; set; } = "Active";

		[Required]
		public Guid UserId { get; set; }
		public int AnnualLeaveBalanceDays { get; set; }


		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		// Navigation
		public List<EducationHistory> Education { get; set; } = new();
		public List<WorkHistory> WorkHistory { get; set; } = new();
		public List<NextOfKin> NextOfKin { get; set; } = new();
		public List<Hobby> Hobbies { get; set; } = new();
		public List<Skill> Skills { get; set; } = new();
		public List<LeaveRequest> LeaveRequests { get; set; } = new();
		public List<SalaryAdvance> SalaryAdvances { get; set; } = new();
		public BankDetail? BankDetail { get; set; }
		public PaymentData? PaymentData { get; set; }
	}
}
