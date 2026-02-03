using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NSwag.Generation;
using PureGym.Application;
using PureGym.SharedKernel.Constants;
using PureGym.WebAPI.Extensions;

var bld = WebApplication.CreateBuilder();
bld.Services.AddApplication();
bld.Services.AddFastEndpoints(o =>
{
    o.Assemblies = [typeof(SwaggerExtensions).Assembly];
});
bld.Services.AddGalleraiSwagger();

var app = bld.Build();

app.UseGalleraiFastEndpoints().UseSwaggerGen();

await app.StartAsync();

try
{
    var generator = app.Services.GetRequiredService<IOpenApiDocumentGenerator>();

    var doc = await generator.GenerateAsync(SwaggerConsts.ApiVersion);

    var json = doc.ToJson();

    var outputPath = Path.Combine(Directory.GetCurrentDirectory(), SwaggerConsts.SwaggerJsonFileName);

    await File.WriteAllTextAsync(outputPath, json);

    Console.WriteLine($"✅ Schema extracted to: {outputPath}");
}
finally
{
    await app.StopAsync();
}
