using System.IO.Compression;
using Calzolari.Grpc.AspNetCore.Validation;
using CountryService.Web.ExternalServices.v1;
using CountryService.Web.Interceptors;
using CountryService.Web.Providers;
using CountryService.Web.Services;
using Grpc.Core;
using Grpc.Net.Compression;

using v1 = CountryService.Web.Services.v1;
using v2 = CountryService.Web.Services.v2;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpc(options =>
{
    options.IgnoreUnknownServices = true;
    options.EnableDetailedErrors = true;
    options.MaxReceiveMessageSize = 6291456; // 6 MB
    options.MaxSendMessageSize = 6291456; // 6 MB

    options.CompressionProviders = new List<ICompressionProvider>
    {
        new BrotliCompressionProvider(CompressionLevel.Optimal)
    };

    options.ResponseCompressionAlgorithm = "br";
    options.ResponseCompressionLevel = CompressionLevel.Optimal;
    options.Interceptors.Add<ExceptionInterceptor>();
    options.EnableMessageValidation();   // Register custom ExceptionInterceptor interceptor
});
builder.Services.AddGrpcReflection();
builder.Services.AddSingleton<ICountryManagementService, CountryManagementService>();
builder.Services.AddSingleton<ProtoService>();
builder.Services.AddGrpcValidation();
builder.Services.AddValidators();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcReflectionService();
app.MapGrpcService<GreeterService>();
app.MapGrpcService<v1.CountryService>();
app.MapGrpcService<v2.CountryService>();

app.MapGet("/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.MapGet("/protos", (ProtoService protoService) => Results.Ok(protoService.GetAll()));

app.MapGet("/protos/v{version:int}/{protoName}", (ProtoService
    protoService, int version, string protoName) =>
{
    var filePath = protoService.Get(version, protoName);
    if (!string.IsNullOrEmpty(filePath))
        return Results.File(filePath);
    
    return Results.NotFound();
});

app.MapGet("/protos/v{version:int}/{protoName}/view", async (ProtoService
    protoService, int version, string protoName) =>
{
    var text = await protoService.ViewAsync(version, protoName);
    if (!string.IsNullOrEmpty(text))
        return Results.Text(text);
    
    return Results.NotFound();
});

app.Use(async (context, next) =>
{
    context.Response.ContentType = "application/grpc";
    context.Response.Headers.Add("grpc-status", ((int)StatusCode.NotFound).ToString());

    await next();
});

app.Run();