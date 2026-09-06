using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.CustomMiddlewares
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _request;
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;

        public ExceptionHandlerMiddleware(
            RequestDelegate request,
            ILogger<ExceptionHandlerMiddleware> logger
        )
        {
            _request = request;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _request.Invoke(context);

                if (
                    context.Response.StatusCode == StatusCodes.Status404NotFound
                    && !context.Response.HasStarted
                )
                {
                    var problem = new ProblemDetails()
                    {
                        Title = "Error while processing HTTP request - end point not found",
                        Status = StatusCodes.Status404NotFound,
                        Detail = $"Endpoint {context.Request.Path} not found",
                        Instance = context.Request.Path,
                    };

                    await context.Response.WriteAsJsonAsync(problem);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Something went wrong");
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                var problem = new ProblemDetails()
                {
                    Title = "An unexpected error occured.",
                    Detail = ex.Message,
                    Status = StatusCodes.Status500InternalServerError,
                    Instance = context.Request.Path,
                };

                await context.Response.WriteAsJsonAsync(problem);
            }
        }
    }
}
