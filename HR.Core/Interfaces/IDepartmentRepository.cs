using HR.Core.Entities;

namespace HR.Core.Interfaces
{
	public interface IDepartmentRepository
	{
		Task<Departmentcore?> GetByIdAsync(Guid id);
		Task<IEnumerable<Departmentcore>> GetAllAsync();
		Task AddAsync(Departmentcore department);
		Task UpdateAsync(Departmentcore department);
		Task DeleteAsync(Guid id);
	}
}
