using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;

namespace CountryServiceClient.Interceptors;

public class TracerInterceptor : Interceptor
{
    private readonly ILogger<TracerInterceptor> _logger;

    public TracerInterceptor(ILogger<TracerInterceptor> logger)
    {
        _logger = logger;
    }
    
    public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context,
        AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
    {
        _logger.LogDebug($"Executing {context.Method.Name} {context.Method.Type} method on service {context.Method.ServiceName}");
        
        return continuation(request, context);
    }

    public override AsyncServerStreamingCall<TResponse> AsyncServerStreamingCall<TRequest, TResponse>(TRequest request,
        ClientInterceptorContext<TRequest, TResponse> context, AsyncServerStreamingCallContinuation<TRequest, TResponse> continuation)
    {
        _logger.LogDebug($"Executing {context.Method.Name} {context.Method.Type} method on service {context.Method.ServiceName}");
        
        return continuation(request, context);
    }

    public override AsyncClientStreamingCall<TRequest, TResponse> AsyncClientStreamingCall<TRequest, TResponse>(ClientInterceptorContext<TRequest, TResponse> context,
        AsyncClientStreamingCallContinuation<TRequest, TResponse> continuation)
    {
        _logger.LogDebug($"Executing {context.Method.Name} {context.Method.Type} method on server {context.Host}");
        
        return continuation(context);
    }

    public override AsyncDuplexStreamingCall<TRequest, TResponse> AsyncDuplexStreamingCall<TRequest, TResponse>(ClientInterceptorContext<TRequest, TResponse> context,
        AsyncDuplexStreamingCallContinuation<TRequest, TResponse> continuation)
    {
        _logger.LogDebug($"Executing {context.Method.Name} {context.Method.Type} method on service {context.Method.ServiceName}");
        
        return continuation(context);
    }
}