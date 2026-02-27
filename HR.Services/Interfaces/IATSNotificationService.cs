using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// HR.Services/Interfaces/IATSNotificationService.cs


namespace HR.Services.Interfaces;

public interface IATSNotificationService
{
	// Triggered when a new application is submitted
	event Action? OnApplicationReceived;
	Task NotifyNewApplicationAsync();
}
