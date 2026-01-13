using HR.Core.Enums;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Services.DTO
{
	public class UnifiedRequestDto
	{
		public Guid Id { get; set; }
		public string RequestType { get; set; } = string.Empty; // "Leave" or "Advance"
		public string Description { get; set; } = string.Empty; // "Annual Leave" or "Salary Advance"
		public string Detail { get; set; } = string.Empty;      // "3 Days" or "$500.00"
		public DateTime Date { get; set; }                      // CreatedAt
		public LeaveStatus Status { get; set; }
	}
}
