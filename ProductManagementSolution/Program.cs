using ProductManagement.API.Extensions;
using ProductManagementAPI.Middleware;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://0.0.0.0:8080");

builder.Host.UseSerilog((context, config) =>
{
    config.ReadFrom.Configuration(context.Configuration);
});

builder.Services
    .AddApplicationServices(builder.Configuration)
    .AddJwtAuthentication(builder.Configuration);

var app = builder.Build();

app.ConfigurePipeline();

app.Run();

public partial class Program;