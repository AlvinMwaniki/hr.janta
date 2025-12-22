using Blazorise;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;

using HR.Data;
using HR.Data.Models.Auth;
using HR.Services;
using HR.Services.Constants;
using HR.Services.Interfaces;
using HR.Services.Services;
using HR.Web.Admin.Blazor.Components;
using HR.Web.Admin.Blazor.Infrastructure;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection; // Needed for CreateScope
using Microsoft.Win32;

using System.IdentityModel.Tokens.Jwt;
using System.Security;
using System.Security.Claims;

System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("Permission");

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents();

builder.Services.AddDbContext<HRDbContext>(options =>
	options.UseMySql(
		builder.Configuration.GetConnectionString("HRDbConnection"),
		new MySqlServerVersion(new Version(8, 0, 43))
	)
);

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
builder.Services.AddScoped<ImpersonationService>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddScoped<CustomAuthenticationStateProvider>();
builder.Services.AddScoped<IPermissionManagementService, PermissionManagementService>();
builder.Services.AddScoped<IUserAccountService, UserAccountService>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddScoped<LeaveService>();
builder.Services.AddScoped<ILeaveService, LeaveService>();
builder.Services.AddScoped<HR.Services.Services.ClaimService>();
builder.Services.AddScoped<IEmployeeDataCacheService, EmployeeDataCacheService>();
builder.Services.AddHttpContextAccessor();


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
app.UseRouting();
app.UseAuthentication(); 
app.UseAuthorization();
app.UseSession();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapStaticAssets();
// ⭐ FINAL, CLEAN FIX: Map the endpoint to the static handler method ⭐
app.MapPost("/api/loginhandler", AuthenticationEndpoints.HandleLoginPostAsync)
	.DisableAntiforgery(); // Disable Antiforgery since Blazor is not managing the form submission
app.MapPost("/logout", (Delegate)AuthenticationEndpoints.HandleLogoutPostAsync);
app.MapRazorComponents<App>()
	.AddInteractiveServerRenderMode();

app.Run();
