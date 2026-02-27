using Blazorise;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;
using Google.GenAI;
using HR.Data;
using HR.Data.Models.Auth;
using HR.Data.Reporting;
using HR.Services;
using HR.Services.Constants;
using HR.Services.Interfaces;
using HR.Services.Services;
using HR.Web.Admin.Blazor.Components;
using HR.Web.Admin.Blazor.Infrastructure;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection; // Needed for CreateScope
using Microsoft.Win32;
using MudBlazor.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Claims;

System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("Permission");

var builder = WebApplication.CreateBuilder(args);

// Path to wkhtmltopdf.exe
var wkhtmlPath = Path.Combine(AppContext.BaseDirectory, "DinkToPdfLibs", "wkhtmltopdf.exe");

// Add services to the container.



builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents();

builder.Services.AddPooledDbContextFactory<HRDbContext>(options =>
	options.UseMySql(
		builder.Configuration.GetConnectionString("HRDbConnection"),
		new MySqlServerVersion(new Version(8, 0, 43))
	));

// This allows services that still expect HRDbContext in their constructor to keep working
builder.Services.AddScoped(p =>
	p.GetRequiredService<IDbContextFactory<HRDbContext>>().CreateDbContext());
builder.Services.AddMemoryCache(); // Required for high speed

builder.Services.AddScoped<DashboardService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<EmployeeTempService>();
builder.Services.AddHttpClient();
builder.Services
	.AddBlazorise(options =>
	{
		options.Immediate = true;
	})
	.AddBootstrap5Providers()
	.AddFontAwesomeIcons();
builder.Services.AddScoped<IAuthService, AuthService>();
// Register the new Email Service
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddServerSideBlazor(options =>
{
	// The default timeout is often too short for debugging.
	options.DetailedErrors = true;
}).AddCircuitOptions(options => {
	options.DetailedErrors = true;
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<CaptchaService>();
builder.Services.AddScoped<ProtectedSessionStorage>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddMudServices();

builder.Services.AddAuthorization(options =>
{
	// Define a general policy based on the role name (standard Blazor/ASP.NET Core)
	options.AddPolicy("RequireAdminRole", policy => policy.RequireRole(AppRoles.Admin));

	options.AddPolicy(AppPermissions.ViewAdminDashboard, policy =>
		policy.RequireClaim("Permission", AppPermissions.ViewAdminDashboard));

	// ⭐ Define policies based on the custom "Permission" claim ⭐
	options.AddPolicy(AppPermissions.ManageUsers, policy =>
		policy.RequireClaim("Permission", AppPermissions.ManageUsers));

	options.AddPolicy(AppPermissions.ViewEmployeeDashboard, policy =>
		policy.RequireClaim("Permission", AppPermissions.ViewEmployeeDashboard));

	options.AddPolicy(AppPermissions.SubmitLeave, policy =>
		policy.RequireClaim("Permission", AppPermissions.SubmitLeave));

	options.AddPolicy(AppPermissions.CanImpersonate, policy =>
	policy.RequireClaim("Permission", AppPermissions.CanImpersonate));


}); 

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<HR.Services.Services.ThemeService>(); 
builder.Services.AddAuthentication(options =>
{
	// The scheme name must match the one you use when creating the ClaimsIdentity ("CustomAuth")
	options.DefaultScheme = "CustomAuth";
	// This tells the framework which scheme to use when challenging (redirecting) an unauthenticated user
	options.DefaultChallengeScheme = "CustomAuth";
})
// Add the authentication handler that uses your custom logic.
// We set up a minimal cookie handler structure, though your CustomAuthenticationStateProvider 
// handles the persistence. This satisfies the framework's requirement for a scheme.
.AddCookie("CustomAuth", options =>
{
	// Configure the redirect URL for unauthenticated access.
	options.LoginPath = "/login";
	options.AccessDeniedPath = "/access-denied";
	options.Cookie.Name = "CustomAuthCookie";
	options.ExpireTimeSpan = TimeSpan.FromDays(30);
	options.SlidingExpiration = true;
});
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
	// Configure session cookie options if needed (e.g., expiry, name)
	options.Cookie.IsEssential = true;
});
builder.Services.AddSingleton<RefreshBroker>(); 
builder.Services.AddScoped<ImpersonationService>();
builder.Services.AddScoped<CustomAuthenticationStateProvider>();
builder.Services.AddScoped<IPermissionManagementService, PermissionManagementService>();
builder.Services.AddScoped<IUserAccountService, UserAccountService>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddScoped<ILeaveService, LeaveService>(); 
builder.Services.AddScoped<HR.Services.Services.ClaimService>();
builder.Services.AddScoped<IEmployeeDataCacheService, EmployeeDataCacheService>();
builder.Services.AddScoped<ILeaveNotificationService, LeaveNotificationService>();
builder.Services.AddScoped<IContractService, ContractService>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<IRequisitionService, RequisitionService>();
builder.Services.AddScoped<JobApplicationService>();
builder.Services.AddSingleton<IATSNotificationService, ATSNotificationService>();
builder.Services.AddSingleton<IAppNotificationService, AppNotificationService>();
builder.Services.AddScoped<IJobApplicationService, JobApplicationService>();
builder.Services.AddScoped<ICvParsingService, CvParsingService>();
builder.Services.AddScoped<IAtsScoringService, AtsScoringService>();
builder.Services.AddScoped<IOnboardingService, OnboardingService>();
builder.Services.AddScoped<IInterviewService, InterviewService>();
builder.Services.AddScoped<ContractPdfService>();





builder.Services.AddHttpClient<OllamaClient>(client =>
{
	client.BaseAddress = new Uri("http://localhost:11434");
});

builder.Services.AddSingleton(sp =>
{
	var configuration = sp.GetRequiredService<IConfiguration>();
	var apiKey = configuration["GoogleGenAi:ApiKey"];

	if (string.IsNullOrEmpty(apiKey))
		throw new InvalidOperationException("GoogleGenAi:ApiKey is missing in configuration.");

	return new Client(apiKey: apiKey);
});

var app = builder.Build();

// Run database seeding immediately after building the app

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error", createScopeForErrors: true);
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
app.MapStaticAssets();
// ⭐ FINAL, CLEAN FIX: Map the endpoint to the static handler method ⭐
app.MapPost("/api/loginhandler", AuthenticationEndpoints.HandleLoginPostAsync)
	.DisableAntiforgery(); // Disable Antiforgery since Blazor is not managing the form submission
app.MapPost("/logout", (Delegate)AuthenticationEndpoints.HandleLogoutPostAsync);
app.MapPost("/api/notify-ats", async (IAppNotificationService adminNotify) =>
{
	await adminNotify.NotifyChangeAsync();
	return Results.Ok();
});
app.MapRazorComponents<App>()
	.AddInteractiveServerRenderMode();
app.Run();
