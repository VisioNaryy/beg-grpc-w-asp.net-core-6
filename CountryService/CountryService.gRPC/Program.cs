using CountryService.DAL.EF.Contexts;
using CountryService.DAL.EF.Repositories;
using CountryService.Domain.Repositories;
using CountryService.gRPC.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

var configuration = builder.Configuration;
var options = configuration.GetConnectionString("DefaultConnection");

// Add services to the container.
services.AddGrpc();
services.AddDbContext<CountryContext>(x =>
{
    x.UseSqlServer(options);
});
services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
services.AddScoped<ICountryRepository, CountryRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>();
app.MapGet("/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();