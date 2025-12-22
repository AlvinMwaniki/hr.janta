using Blazorise;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;

using HR.Data;
using HR.Services;
using HR.Services.Services;

using HR.Web.Admin;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

//======================
// logs:
builder.Logging.SetMinimumLevel(LogLevel.Debug);
builder.Logging.AddConsole();
builder.Logging.AddDebug();
// =======================
// 1️⃣ Serilog Configuration
// =======================
Log.Logger = new LoggerConfiguration()
.MinimumLevel.Debug()
.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
.Enrich.FromLogContext()
.WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Information)
.WriteTo.File("Logs/Info/log-.txt", rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: LogEventLevel.Information)
.WriteTo.File("Logs/Error/log-.txt", rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: LogEventLevel.Error)
.WriteTo.File("Logs/Fatal/log-.txt", rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: LogEventLevel.Fatal)
.CreateLogger();

builder.Host.UseSerilog();

// =======================
// 2️⃣ Add Services
// =======================

// Blazorise setup for Server
builder.Services
	.AddBlazorise(options =>
	{
		options.Immediate = true; // similar to ChangeTextOnKeyPress
	})
	.AddBootstrap5Providers()
	.AddFontAwesomeIcons();  // optional but required for icons
builder.Services.AddRazorPages();
builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<DashboardService>();
builder.Services.AddScoped<EmployeeTempService>();
builder.Services.AddScoped<IAuthService, AuthService>();
// Register the Email Service
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<CaptchaService>();
builder.Services.AddScoped<ProtectedSessionStorage>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthorization();
// Register Custom Authentication Provider
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();


// EF Core: HRDbContext
builder.Services.AddDbContext<HRDbContext>(options =>
options.UseMySql(
	builder.Configuration.GetConnectionString("HRDbConnection"),
	new MySqlServerVersion(new Version(8, 0, 43))
	)
);

// =======================
// 3️⃣ Build App
// =======================
var app = builder.Build();
// Middleware
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error");
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapBlazorHub(); // Blazor Server
app.MapFallbackToPage("/_Host");

app.Run();