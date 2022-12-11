using System.IO.Compression;
using CountryService.DAL.EF.Contexts;
using CountryService.DAL.EF.Repositories;
using CountryService.Domain.Repositories;
using CountryService.Domain.Services;
using CountryService.gRPC.Interceptors;
using CountryService.gRPC.Providers;
using CountryService.gRPC.Services;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

var configuration = builder.Configuration;
var options = configuration.GetConnectionString("DefaultConnection");

// Add services to the container.
services.AddGrpc(options =>
{
    options.EnableDetailedErrors = true;
    options.IgnoreUnknownServices = true;
    options.MaxReceiveMessageSize = 6291456; // 6 MB
    options.MaxSendMessageSize = 6291456; // 6 MB
    
    options.CompressionProviders = new List<Grpc.Net.Compression.ICompressionProvider>
    {
        new CustomBrotliCompressionProvider(CompressionLevel.Optimal) // br
    };
    
    options.ResponseCompressionAlgorithm = "br"; // grpc-accept-encoding
    options.ResponseCompressionLevel = CompressionLevel.Optimal; // compression level used if not set on the provider
    options.Interceptors.Add<ExceptionInterceptor>(); // Register custom ExceptionInterceptor interceptor
});
builder.Services.AddGrpcReflection();
builder.Services.AddScoped<ICountryRepository, CountryRepository>();
builder.Services.AddScoped<ICountryService, CountryService.BLL.Services.CountryService>();
//builder.Services.AddSingleton<ProtoService>();
services.AddDbContext<CountryContext>(x =>
{
    x.UseSqlServer(options);
});
services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
services.AddScoped<ICountryRepository, CountryRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcReflectionService();
app.MapGrpcService<GreeterService>();
app.MapGrpcService<CountryGrpcService>();
app.MapGet("/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();