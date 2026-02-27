using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Services.DTO
{
	public class NotificationDto
	{
		public Guid Id { get; set; } = Guid.NewGuid();
		public string Message { get; set; } = string.Empty;
		public string TargetUrl { get; set; } = string.Empty;
		public DateTime CreatedAt { get; set; } = DateTime.Now;
		public bool IsRead { get; set; }
		public string? Type { get; set; } // e.g., "Leave", "Advance", "System"
	}
}
