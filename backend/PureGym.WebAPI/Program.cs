using FastEndpoints;
using PureGym.Application;
using PureGym.Infrastructure;
using PureGym.WebAPI.Extensions;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddHealthChecks();

builder.Services.AddFastEndpoints();
builder.Services.AddGalleraiSwagger();

builder.Services.AddOpenApi();

var app = builder.Build();

await app.UseApplyMigrations();
await app.UseDatabaseSeed();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapHealthChecks("/health");
//app.UseAuthentication();
//app.UseAuthorization();

app.UseGalleraiFastEndpoints();

app.Run();