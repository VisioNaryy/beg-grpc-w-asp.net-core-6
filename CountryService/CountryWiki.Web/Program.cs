
using System.IO.Compression;
using CountryWiki.BLL.Services;
using CountryWiki.DAL.Repositories;
using CountryWiki.DAL.v1;
using CountryWiki.Domain.Repositories;
using CountryWiki.Domain.Services;
using CountryWiki.Web.BackgroundServices;
using CountryWiki.Web.Channels;
using CountryWiki.Web.Interceptors;
using CountryWiki.Web.Options;
using CountryWiki.Web.Providers;
using Grpc.Net.Client.Web;
using Microsoft.AspNetCore.ResponseCompression;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<ICountryRepository, CountryRepository>();
builder.Services.AddScoped<ICountryService, CountryService>();
builder.Services.AddScoped<ICountryFileUploadValidatorService, CountryFileUploadValidatorService>();
builder.Services.AddSingleton<ISyncCountriesChannel, SyncCountriesChannel>();
builder.Services.AddHostedService<SyncUploadedCountriesBackgroundService>();
builder.Services.AddSingleton(new GlobalOptions
{
    ProcessingUpload = false
});

var loggerFactory = LoggerFactory.Create(logging =>
{
    logging.AddConsole();
    logging.SetMinimumLevel(LogLevel.Trace);
});

var section = builder.Configuration.GetSection("CountryGrpcServiceUri").Value;

builder.Services.AddGrpcClient<CountryGrpc.CountryGrpcClient>(o =>
    {
        o.Address = new Uri(builder.Configuration.GetSection("CountryGrpcServiceUri").Value);
    })
    .ConfigurePrimaryHttpMessageHandler(() => new GrpcWebHandler(new HttpClientHandler()))
    .AddInterceptor(() => new TracerInterceptor(loggerFactory.CreateLogger<TracerInterceptor>()))
    .ConfigureChannel(o =>
    {
        o.CompressionProviders = new List<Grpc.Net.Compression.ICompressionProvider>
        {
            new CustomBrotliCompressionProvider(CompressionLevel.Optimal)
        };
        o.MaxReceiveMessageSize = 6291456; // 6 MB,
        o.MaxSendMessageSize = 6291456; // 6 MB
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();