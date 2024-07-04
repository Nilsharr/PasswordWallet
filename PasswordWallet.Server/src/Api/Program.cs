using System.Text.Json;
using System.Text.Json.Serialization;
using Api.Extensions;
using Core.Constants;
using Core.Extensions;
using FastEndpoints;
using FastEndpoints.Swagger;
using Infrastructure.Extensions;
using Infrastructure.Options;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var corsOptions = builder.Configuration.GetRequiredSection(CorsOptions.SectionName).Get<CorsOptions>() ??
                  throw new InvalidOperationException();

builder.Host.UseSerilog((_, configuration) => { configuration.ReadFrom.Configuration(builder.Configuration); });

builder.Services.AddFastEndpoints();
builder.Services.AddCors(options => options.AddPolicy(corsOptions.PolicyName,
    b => b.WithOrigins(corsOptions.Origins).AllowAnyMethod().AllowAnyHeader()));
builder.Services.AddCoreServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddSwaggerDoc();
}

var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseDefaultExceptionHandler();

app.UseFastEndpoints(c =>
{
    c.Endpoints.RoutePrefix = RouteConstants.RoutePrefix;
    c.Versioning.Prefix = "v";
    c.Versioning.DefaultVersion = 1;
    c.Versioning.PrependToRoute = true;
    c.Endpoints.ShortNames = true;
    c.Serializer.Options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    c.Serializer.Options.Converters.Add(new JsonStringEnumConverter());
});

if (app.Environment.IsDevelopment())
{
    await app.MigrateDatabase();
    app.UseSwaggerGen();
}

app.UseHttpsRedirection()
    .UseCors(corsOptions.PolicyName)
    .UseAuthentication()
    .UseAuthorization();

app.Run();

public partial class Program;