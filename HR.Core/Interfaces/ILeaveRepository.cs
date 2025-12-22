using HR.Core.Entities;

namespace HR.Core.Interfaces
{
	public interface ILeaveRepository
	{
		Task<Leavecore?> GetByIdAsync(Guid id);
		Task<IEnumerable<Leavecore>> GetAllAsync();
		Task AddAsync(Leavecore leave);
		Task UpdateAsync(Leavecore leave);
		Task DeleteAsync(Guid id);

		Task<IEnumerable<Leavecore>> GetByEmployeeAsync(Guid employeeId);
	}
}
