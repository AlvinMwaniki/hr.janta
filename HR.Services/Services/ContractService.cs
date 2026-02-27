using HR.Core.Enums;
using HR.Data;
using HR.Data.Models.EmployestatusLog;
using HR.Services.Interfaces;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Services.Services
{
	public class ContractService : IContractService
	{
		private readonly HRDbContext _context;

		public ContractService(HRDbContext context)
		{
			_context = context;
		}

		public async Task ProcessExpiredContractsAsync()
		{
			var today = DateTime.Today;

			// 1. Find all ACTIVE employees whose contract date has passed
			var expiredEmployees = await _context.Employees
				.Where(e => e.Status == EmployeeStatus.Active
						 && e.ContractEndDate != null
						 && e.ContractEndDate <= today)
				.ToListAsync();

			if (!expiredEmployees.Any()) return;

			foreach (var emp in expiredEmployees)
			{
				// 2. Change the Status
				emp.Status = EmployeeStatus.ContractEnded;

				// 3. Log the change for History (Audit Trail)
				_context.EmployeeStatusLogs.Add(new EmployeeStatusLog
				{
					Id = Guid.NewGuid(),
					EmployeeId = emp.Id,
					NewStatus = EmployeeStatus.ContractEnded,
					EffectiveDate = DateTime.Now,
					Reason = "Automated: Contract period reached.",
					AuthorizedBy = "SYSTEM_BOT",
					Notes = $"Contract expired on {emp.ContractEndDate:dd MMM yyyy}"
				});
			}

			// 4. Save all changes at once
			await _context.SaveChangesAsync();
		}
	}
}
