using AppointmentScheduler.Components;
using AppointmentScheduler.Data;
using AppointmentScheduler.Services;
using Microsoft.EntityFrameworkCore;
using Syncfusion.Blazor;

var builder = WebApplication.CreateBuilder(args);

// Add Syncfusion License Key (get from Syncfusion Community License)
Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Your-Syncfusion-License-Key-Here");

// Add Blazor components
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Register Services
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<AppointmentService>();
builder.Services.AddScoped<CalendarService>();

// Add HttpClient for API communication
builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiService:Url"] ?? "http://localhost:5001");
});

// Add Entity Framework (if using database)
builder.Services.AddDbContext<AppointmentContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Syncfusion Blazor
builder.Services.AddSyncfusionBlazor();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>();

app.Run();