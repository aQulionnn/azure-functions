using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Logging;

namespace CurrencyConverter.Functions.Middlewares;

public class ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger) : IFunctionsWorkerMiddleware
{
    private readonly ILogger<ExceptionHandlingMiddleware> _logger = logger;
    
    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        try
        {
            _logger.LogInformation("Executing function {functionName}", nameof(context.FunctionDefinition.Name));
            await next(context);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }
    }
}