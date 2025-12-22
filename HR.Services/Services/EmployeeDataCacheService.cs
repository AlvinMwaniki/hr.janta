// HR.Services/Services/EmployeeDataCacheService.cs

public class EmployeeDataCacheService : IEmployeeDataCacheService
{
	public Guid EmployeeId { get; private set; } = Guid.Empty;

	public void SetEmployeeId(Guid id)
	{
		EmployeeId = id;
	}
}