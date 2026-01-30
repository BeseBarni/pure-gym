using MassTransit;
using PureGym.Mock.RevolvingDoor;
using PureGym.SharedKernel.Events;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<DoorState>();
builder.Services.AddOpenApi();
builder.Services.AddGeneratedSettings(builder.Configuration);

var rabbitMqSettings =
    builder.Configuration.GetSection(RabbitMQSettings.SectionName)
    .Get<RabbitMQSettings>()
    ?? throw new InvalidOperationException("RabbitMQ settings are not configured");

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<GymAccessGrantedConsumer>();
    x.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(rabbitMqSettings.Host, "/", h =>
        {
            h.Username(rabbitMqSettings.UserName);
            h.Password(rabbitMqSettings.Password);
        });
        cfg.ReceiveEndpoint("revolving-door", e =>
        {
            e.ConfigureConsumer<GymAccessGrantedConsumer>(ctx);
            e.Bind<GymAccessGrantedEvent>(); // Bind to the message exchange
        });
    });
});

builder.Services.AddHealthChecks();
builder.Services.AddControllers();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapHealthChecks("/health");
app.MapControllers();
app.Run();

