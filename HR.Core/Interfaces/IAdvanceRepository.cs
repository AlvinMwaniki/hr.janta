using HR.Core.Entities;

namespace HR.Core.Interfaces
{
	public interface IAdvanceRepository
	{
		Task<Advancecore?> GetByIdAsync(Guid id);
		Task<IEnumerable<Advancecore>> GetAllAsync();
		Task AddAsync(Advancecore advance);
		Task UpdateAsync(Advancecore advance);
		Task DeleteAsync(Guid id);

		Task<IEnumerable<Advancecore>> GetByEmployeeAsync(Guid employeeId);
	}
}
