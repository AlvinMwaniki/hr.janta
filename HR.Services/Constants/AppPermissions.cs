// Example: HR.Services/Constants/AppPermissions.cs

namespace HR.Services.Constants;

public static class AppPermissions
{
	// Admin/HR Permissions
	public const string ManageUsers = "Permissions.Users.Manage";      // Admin can give rights
	public const string ViewAdminDashboard = "Permissions.Dashboard.AdminView";
	public const string ViewAllLeaveRequests = "Permissions.Dashboard.AdminView";
	public const string ApproveRejectLeave = "Permissions.Dashboard.AdminView";
	public const string EditEmployeeProfiles = "Permissions.Dashboard.AdminView";
	public const string ManagePayrollData = "Permissions.Dashboard.AdminView";
	public const string ManageRoles = "Permissions.Dashboard.AdminView";
	public const string ViewReports = "Permissions.Dashboard.AdminView";
	public const string CanImpersonate = "Permissions.Admin.CanImpersonate";

	// Employee/General Permissions
	public const string SubmitLeave = "Permissions.Leave.Submit";    // Default right for all employees
	public const string ViewEmployeeDashboard = "Permissions.Dashboard.EmployeeView";
	public const string ViewPayslips = "Permissions.Dashboard.EmployeeView";


}