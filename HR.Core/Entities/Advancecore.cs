namespace HR.Core.Entities
{
	public class Advancecore
	{
		public Guid Id { get; set; }
		public Guid EmployeeId { get; set; }

		public decimal Amount { get; set; }
		public DateTime RequestedOn { get; set; }

		public Employeecore? Employee { get; set; }
	}
}
