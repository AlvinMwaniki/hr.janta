using HR.Data.Models.Advances;
using HR.Data.Models.BANKING;
using HR.Data.Models.Departments;
using HR.Data.Models.Leaves;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HR.Data.Models.Employees
{
	public class Employee
	{
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
		public string Email { get; set; } = default!;

		[Required(ErrorMessage = "Phone number is required")]
		public string Phone { get; set; } = default!;

		[Required(ErrorMessage = "Address is required")]
		public string Address { get; set; } = default!;

		[Required(ErrorMessage = "Date of Birth is required")]
		public DateTime DOB { get; set; }

		[Required(ErrorMessage = "National ID is required")]
		public string? NationalID { get; set; }

		[Required(ErrorMessage = "Gender is required")]
		public string? Gender { get; set; }

		[Required(ErrorMessage = "Job Title is required")]
		public string? JobTitle { get; set; } // NEW

		[Required(ErrorMessage = "Disability is required")]
		public string? Disability { get; set; }

		// Ethnicity is optional
		public string? Ethnicity { get; set; }

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
