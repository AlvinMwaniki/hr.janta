// HR.Services/Interfaces/IEmployeeDataCacheService.cs

public interface IEmployeeDataCacheService
{
	Guid EmployeeId { get; }
	void SetEmployeeId(Guid id);
}