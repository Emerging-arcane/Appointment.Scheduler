

var builder = DistributedApplication.CreateBuilder(args);

// Add the Blazor Web App
var blazorApp = builder.AddProject<Projects.AppointmentScheduler>("appointment-scheduler");

builder.Build().Run();