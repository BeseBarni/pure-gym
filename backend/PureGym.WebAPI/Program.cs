using FastEndpoints;
using FastEndpoints.Swagger;
using PureGym.Application;
using PureGym.Infrastructure;
using PureGym.WebAPI.Extensions;

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
    app.UseSwaggerGen();
    app.UseSwaggerUi();
}

app.MapHealthChecks("/health");
app.UseAuthentication();
app.UseAuthorization();

app.UseGalleraiFastEndpoints();

app.Run();