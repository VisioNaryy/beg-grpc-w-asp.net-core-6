﻿using System.IO.Compression;
using CountryServiceClient.Interceptors;
using CountryServices.v1;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using CountryServiceClientV1 = CountryServices.v1.CountryService.CountryServiceClient;
using CountryServiceClientV2 = CountryServices.v2.CountryService.CountryServiceClient;
using ICompressionProvider = Grpc.Net.Compression.ICompressionProvider;
using CustomBrotliProvider = CountryServiceClient.Providers.BrotliCompressionProvider;

namespace CountryServiceClient;

public class App
{
    private readonly IConfiguration _configuration;

    public App(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task Run()
    {
        var httpHandler = new HttpClientHandler
        {
            // Return true to allow certificates that are untrusted/invalid
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
            UseProxy = false
        };

        var sockerHttpHandler = new SocketsHttpHandler
        {
            KeepAlivePingDelay = TimeSpan.FromSeconds(15),
            PooledConnectionIdleTimeout = TimeSpan.FromMinutes(5), // Timeout.InfiniteTimeSpan for infinite idle connection
            KeepAlivePingTimeout = TimeSpan.FromSeconds(5),
            EnableMultipleHttp2Connections = true
        };

        var loggerFactory = LoggerFactory.Create(logging =>
        {
            logging.AddConsole();
            logging.SetMinimumLevel(LogLevel.Trace);
        });

        var grpcChannelOptions = new GrpcChannelOptions
        {
            LoggerFactory = loggerFactory,
            HttpHandler = httpHandler,
            CompressionProviders = new List<ICompressionProvider>
            {
                new CustomBrotliProvider(CompressionLevel.Optimal)
            },
            MaxReceiveMessageSize = 6291456, // 6 MB,
            MaxSendMessageSize = 6291456 // 6 MB
        };

        var address = _configuration.GetValue<string>("CountryService:Url");
        
        var logger = loggerFactory.CreateLogger<TracerInterceptor>();

        using var channel = GrpcChannel.ForAddress(address, grpcChannelOptions);

        var countryClient = new CountryServiceClientV1(channel.Intercept(new TracerInterceptor(logger)));

        // await GetAll(countryClient);
        // await Delete(countryClient);
        // await Create(countryClient);
        await Get(countryClient);
    }

    private async Task GetAll(CountryServiceClientV1 client)
    {
        using (var serverStreamingCall = client.GetAll(new Empty()))
        {
            await foreach (var response in serverStreamingCall.ResponseStream.ReadAllAsync())
            {
                Console.WriteLine($"{response.Name}: {response.Description}");
            }

            // Read headers and trailers
            var serverStreamingCallHeaders = await serverStreamingCall.ResponseHeadersAsync;
            var serverStreamingCallTrailers = serverStreamingCall.GetTrailers();

            var myHeaderValue = serverStreamingCallHeaders.GetValue("myHeaderName");
            var myTrailerValue = serverStreamingCallTrailers.GetValue("myTrailerName");
        }
    }

    private async Task Delete(CountryServiceClientV1 client)
    {
        using var clientStreamingCall = client.Delete();
        var countriesToDelete = new List<CountryIdRequest>
        {
            new CountryIdRequest
            {
                Id = 1
            },
            new CountryIdRequest
            {
                Id = 2
            }
        };

        // Write
        foreach (var countryToDelete in countriesToDelete)
        {
            await clientStreamingCall.RequestStream.WriteAsync(countryToDelete);
            Console.WriteLine($"Country with Id {countryToDelete.Id} set for deletion");
        }

        // Tells server that request streaming is done
        await clientStreamingCall.RequestStream.CompleteAsync();

        // Finish the call by getting the response
        var emptyResponse = await clientStreamingCall.ResponseAsync;

        // Read headers and Trailers
        var clientStreamingCallHeaders = await clientStreamingCall.ResponseHeadersAsync;
        var clientStreamingCallTrailers = clientStreamingCall.GetTrailers();
        var myHeaderValue = clientStreamingCallHeaders.GetValue("myHeaderName");
        var myTrailerValue = clientStreamingCallTrailers.GetValue("myTrailerName");
    }

    private async Task Create(CountryServiceClientV1 client)
    {
        using var bidirectionnalStreamingCall = client.Create();
        var countriesToCreate = new List<CountryCreationRequest>
        {
            new CountryCreationRequest
            {
                Name = "France",
                Description = "Western european country",
                CreateDate = Timestamp.FromDateTime(DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc))
            },
            new CountryCreationRequest
            {
                Name = "Poland",
                Description = "Eastern european country",
                CreateDate = Timestamp.FromDateTime(DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc))
            }
        };
        
        // Write
        foreach (var countryToCreate in countriesToCreate)
        {
            await bidirectionnalStreamingCall.RequestStream.WriteAsync(countryToCreate);
            Console.WriteLine($"Country {countryToCreate.Name} set for creation");
        }
        
        // Tells server that request streaming is done
        await bidirectionnalStreamingCall.RequestStream.CompleteAsync();
        
        // Read
        await foreach (var createdCountry in bidirectionnalStreamingCall.ResponseStream.ReadAllAsync())
        {
            Console.WriteLine($"{createdCountry.Name} has been created with Id: {createdCountry.Id}");
        }
        
        // Read headers and Trailers
        var bidirectionnalStreamingCallHeaders = await bidirectionnalStreamingCall.ResponseHeadersAsync;
        var bidirectionnalStreamingCallTrailers = bidirectionnalStreamingCall.GetTrailers();
        
        var myHeaderValue = bidirectionnalStreamingCallHeaders.GetValue("myHeaderName");
        var myTrailerValue = bidirectionnalStreamingCallTrailers.GetValue("myTrailerName");
    }
    
    private async Task Get(CountryServiceClientV1 client)
    {
        var countryIdRequest = new CountryIdRequest { Id = 1 };
        
        try
        {
            var countryCall = client.GetAsync(countryIdRequest, deadline: DateTime.UtcNow.AddSeconds(30));
            var country = await countryCall.ResponseAsync;
        
            Console.WriteLine($"{country.Id}: {country.Name}");
        
            // Read headers and Trailers
            var countryCallHeaders = await countryCall.ResponseHeadersAsync;
            var countryCallTrailers = countryCall.GetTrailers();
            var myHeaderValue = countryCallHeaders.GetValue("myHeaderName");
            var myTrailerValue = countryCallTrailers.GetValue("myTrailerName");
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.DeadlineExceeded)
        {
            Console.WriteLine($"Get country with Id: {countryIdRequest.Id} has timed out");
            var trailers = ex.Trailers;
            var correlationId = trailers.GetValue("correlationId");
        }
        catch (RpcException ex)
        {
            Console.WriteLine($"An error occured while getting the country with Id: {countryIdRequest.Id}");
            var trailers = ex.Trailers;
            var correlationId = trailers.GetValue("correlationId");
        }
    }
}