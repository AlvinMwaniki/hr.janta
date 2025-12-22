

using HR.Core.Enums;

namespace HR.Core.Entities
{
	public class Employeecore
	{
		public Guid Id { get; set; }
		public string EmployeeCode { get; set; } = default!;

		public string FirstName { get; set; } = default!;
		public string? MiddleName { get; set; } 
		public string LastName { get; set; } = default!;

		public string Email { get; set; } = default!;
		public string Phone { get; set; } = default!;

		public string Address { get; set; } = default!;
		public DateTime DOB { get; set; }

		public string? NationalID { get; set; }
		public string? Gender { get; set; }
		public string? JobTitle { get; set; }

		public ContractType ContractType { get; set; } = ContractType.Permanent;

		public EmployeeStatus Status { get; set; } = EmployeeStatus.Active;

		public Guid DepartmentId { get; set; }
		public Departmentcore? Department { get; set; }

		public DateTime HireDate { get; set; }

		// Related business operations
		public List<Leavecore> Leaves { get; set; } = new();
		public List<Advancecore> Advances { get; set; } = new();

	}
}
