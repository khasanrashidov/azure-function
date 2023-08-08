using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace UtilizationReports.FunctionApp.Middlewares
{
    public class CustomExceptionHandlerMiddleware : IFunctionsWorkerMiddleware
    {
        private readonly ILogger<CustomExceptionHandlerMiddleware> _logger;

        public CustomExceptionHandlerMiddleware(ILogger<CustomExceptionHandlerMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {
            try
            {
                _logger.LogInformation("Executing function: {FunctionName}", context.FunctionDefinition.Name);
                await next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception has occurred while executing the function.");
            }
        }
    }

    public static class CustomExceptionHandlerMiddlewareExtensions
    {
        public static IFunctionsWorkerApplicationBuilder UseCustomExceptionHandler(this IFunctionsWorkerApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomExceptionHandlerMiddleware>();
        }
    }
}
