using Grpc.Core;
using Microsoft.Data.SqlClient;

namespace CountryService.Web.Extensions;

public static class ExceptionExtensions
{
    public static RpcException Handle<T>(this Exception exception, ServerCallContext context, ILogger<T> logger,
        Guid correlationId)
    =>
        exception switch
        {
            TimeoutException => HandleTimeoutException(exception as TimeoutException, context, logger, correlationId),
            SqlException => HandleSqlException(exception as SqlException, context, logger, correlationId),
            RpcException => HandleRpcException(exception as RpcException, context, logger, correlationId),
            _ => HandleDefault(exception, context, logger, correlationId)
        };

    private static RpcException HandleTimeoutException<T>(TimeoutException? exception, ServerCallContext context,
        ILogger<T> logger, Guid correlationId)
    {
        logger.LogError(exception, $"CorrelationId: {correlationId} - A timeout occurred");
        
        var status = new Status(StatusCode.Internal, "An external resource did not answer within the time limit");

        return new RpcException(status, CreateTrailers(correlationId));
    }

    private static RpcException HandleSqlException<T>(SqlException? exception, ServerCallContext context,
        ILogger<T> logger, Guid correlationId)
    {
        logger.LogError(exception, $"CorrelationId: {correlationId} - An SQL error occurred");
        
        Status status;
        if (exception.Number == -2)
        {
            status = new Status(StatusCode.DeadlineExceeded, "SQL timeout");
        }
        else
        {
            status = new Status(StatusCode.Internal, "SQL error");
        }

        return new RpcException(status, CreateTrailers(correlationId));
    }

    private static RpcException HandleRpcException<T>(RpcException? exception, ServerCallContext context,
        ILogger<T> logger, Guid
            correlationId)
    {
        logger.LogError(exception, $"CorrelationId: {correlationId} - An error occurred");

        var trailers = new Metadata();
        
        foreach (var trailer in exception.Trailers)
        {
            trailers.Add(trailer);
        }
        
        trailers.Add(CreateTrailers(correlationId).FirstOrDefault());

        var status = new Status(exception.StatusCode, exception.Message);

        return new RpcException(status, trailers);
    }
    
    private static RpcException HandleDefault<T>(Exception? exception,
        ServerCallContext context, ILogger<T> logger, Guid correlationId)
    {
        logger.LogError(exception, $"CorrelationId: {correlationId} - An error occurred");

        var status = new Status(StatusCode.Internal, exception.Message);

        return new RpcException(status, CreateTrailers(correlationId));
    }

    /// <summary>
    /// Adding the correlation to Response Trailers
    /// </summary>
    /// <param name="correlationId"></param>
    /// <returns></returns>
    private static Metadata CreateTrailers(Guid correlationId)
    {
        var trailers = new Metadata();
        trailers.Add("CorrelationId", correlationId.ToString());

        return trailers;
    }
}