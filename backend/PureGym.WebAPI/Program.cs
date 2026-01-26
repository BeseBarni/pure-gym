using FastEndpoints;
using FastEndpoints.Swagger;
using FitNetClean.Infrastructure;
using Microsoft.EntityFrameworkCore;
using PureGym.Application;
using PureGym.WebAPI.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddGeneratedSettings(builder.Configuration);

builder.Services.AddHealthChecks();

builder.Services.AddFastEndpoints();
builder.Services.SwaggerDocument(o =>
{
    o.DocumentSettings = s =>
    {
        s.Title = "PureGym API";
        s.Version = "v1";
        s.Description = "API for PureGym management system";
    };
});
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

await app.UseApplyMigrations();
await app.UseDatabaseSeed();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapHealthChecks("/health");
app.UseAuthentication();
app.UseAuthorization();

app.UseFastEndpoints(c =>
{
    c.Endpoints.RoutePrefix = "api";
});
app.UseSwaggerGen();

app.Run();