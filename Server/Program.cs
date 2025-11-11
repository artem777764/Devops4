using System.Net;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Server.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5123, lo => lo.Protocols = HttpProtocols.Http2);
});

var app = builder.Build();

app.MapGrpcReflectionService();

app.MapGrpcService<GreeterService>();

app.Run();