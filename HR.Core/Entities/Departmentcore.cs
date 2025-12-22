using HR.Core.Enums;

namespace HR.Core.Entities
{
	public class Departmentcore
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = default!;
		public string Code { get; set; } = default!;

		public DepartmentType DepartmentType { get; set; }


		public List<Employeecore> Employees { get; set; } = new();
	}
}
