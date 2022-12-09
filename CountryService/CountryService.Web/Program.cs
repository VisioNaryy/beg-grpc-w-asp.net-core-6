using System.IO.Compression;
using CountryService.Web.ExternalServices;
using CountryService.Web.Interceptors;
using CountryService.Web.Providers;
using CountryService.Web.Services;
using Grpc.Core;
using Grpc.Net.Compression;
using CountryServiceClass = CountryService.Web.Services.CountryService;

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
});
builder.Services.AddGrpcReflection();
builder.Services.AddSingleton<ICountryManagementService, CountryManagementService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcReflectionService();
app.MapGrpcService<GreeterService>();
app.MapGrpcService<CountryServiceClass>();
app.MapGet("/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Use(async (context, next) =>
{
    context.Response.ContentType = "application/grpc";
    context.Response.Headers.Add("grpc-status", ((int)StatusCode.NotFound).ToString());

    await next();
});

app.Run();