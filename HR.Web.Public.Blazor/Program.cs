using Azure.Core;

using Google.GenAI;

using HR.Data;
using HR.Services.Interfaces;
using HR.Services.Services;
using HR.Web.Public.Blazor.Components;
using HR.Web.Public.Blazor.Components.Services;
using HR.Web.Public.Infrastructure;

using Microsoft.EntityFrameworkCore;

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
builder.Services.AddScoped<IRequisitionService, RequisitionService>();
builder.Services.AddScoped<ICurrentUserService, PublicGuestService>();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<RefreshBroker>();
builder.Services.AddScoped<IAppNotificationService, PublicNotificationService>();
builder.Services.AddScoped<IJobApplicationService, JobApplicationService>();
builder.Services.AddSingleton(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var apiKey = configuration["GoogleGenAi:ApiKey"];

    if (string.IsNullOrEmpty(apiKey))
        throw new InvalidOperationException("GoogleGenAi:ApiKey is missing in configuration.");

    return new Client(apiKey: apiKey);
});

// Then register your CvParsingService
builder.Services.AddScoped<ICvParsingService, CvParsingService>();
builder.Services.AddScoped<IAtsScoringService, AtsScoringService>();
builder.Services.AddScoped<IJobApplicationService, JobApplicationService>();
builder.Services.AddScoped<IInterviewService, InterviewService>();
builder.Services.AddScoped<IOnboardingService, OnboardingService>();
builder.Services.AddScoped<EmployeeTempService>();
builder.Services.AddScoped<ContractPdfService>();

builder.Services.AddHttpClient<OllamaClient>(client =>
{
	client.BaseAddress = new Uri("http://localhost:11434");
});

var app = builder.Build();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error", createScopeForErrors: true);
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
	.AddInteractiveServerRenderMode();

app.Run();
