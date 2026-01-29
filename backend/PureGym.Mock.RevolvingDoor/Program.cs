using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PureGym.Mock.RevolvingDoor;
using PureGym.SharedKernel.DTOs;
using Scalar.AspNetCore;

const string GYM_API = "GymApi";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddGeneratedSettings(builder.Configuration);

builder.Services.AddHttpClient(GYM_API, (serviceProvider, client) =>
{
    var settings = serviceProvider
        .GetRequiredService<IOptions<GymApiSettings>>().Value;

    client.BaseAddress = new Uri(settings.BaseUrl);
    client.Timeout = TimeSpan.FromSeconds(settings.TimeoutSeconds);
});
builder.Services.AddHealthChecks();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapHealthChecks("/health");

app.MapPost("/scan", async ([FromBody] VerifyAccessRequest request, IHttpClientFactory factory, IOptions<GymApiSettings> options) =>
{
    var settings = options.Value;
    var client = factory.CreateClient(GYM_API);

    var response = await client.PostAsJsonAsync(settings.VerifyEndpoint, request);

    if (!response.IsSuccessStatusCode)
        return Results.Forbid();

    Console.WriteLine($"[HARDWARE] QR {request.MemberId} {request.AccessKey} Validated. Door Unlocked!");
    return Results.Ok(new { Message = "Door Opened" });
});

app.Run();

