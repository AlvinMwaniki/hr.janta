using HR.Core.Entities;

namespace HR.Core.Interfaces
{
	public interface IEmployeeRepository
	{
		Task<Employeecore?> GetByIdAsync(Guid id);
		Task<IEnumerable<Employeecore>> GetAllAsync();
		Task AddAsync(Employeecore employee);
		Task UpdateAsync(Employeecore employee);
		Task DeleteAsync(Guid id);

		// Extra useful filters
		Task<IEnumerable<Employeecore>> GetByDepartmentAsync(Guid departmentId);
		Task<IEnumerable<Employeecore>> SearchAsync(string keyword);
	}
}
