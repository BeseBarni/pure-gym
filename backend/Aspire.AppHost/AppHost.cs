var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithPgAdmin();

var puregymdb = postgres.AddDatabase("puregymdb");

builder.AddProject<Projects.PureGym_WebAPI>("puregym-webapi")
    .WithReference(puregymdb)
    .WaitFor(postgres);

builder.Build().Run();
