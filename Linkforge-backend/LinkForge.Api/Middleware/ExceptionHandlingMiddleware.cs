using System.Text.Json;
using LinkForge.Application.Exceptions;
    
namespace LinkForge.Api.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ValidationException ex)
            {
                await WriteErrorResponseAsync(
                    context,
                    StatusCodes.Status400BadRequest,
                    ex.Message);
            }
            catch (ConflictException ex)
            {
                await WriteErrorResponseAsync(
                    context,
                    StatusCodes.Status409Conflict,
                    ex.Message);
            }
            catch (NotFoundException ex)
            {
                await WriteErrorResponseAsync(
                    context,
                    StatusCodes.Status404NotFound,
                    ex.Message);
            }
        }

        private static async Task WriteErrorResponseAsync(
            HttpContext context,
            int statusCode,
            string message)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var response = new { message };

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response));
        }
    }
}
