using GenericApiResponse.Models;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;

namespace GenericApiResponse.Middleware
{
    /// <summary>
    /// Middleware that globally handles unhandled exceptions in the ASP.NET Core pipeline.
    /// Converts all exceptions into a standardized <see cref="ApiResponse{T}"/> format for consistent API responses.
    /// </summary>
    internal class ApiResponseExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiResponseExceptionMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next <see cref="RequestDelegate"/> in the pipeline.</param>
        public ApiResponseExceptionMiddleware(RequestDelegate next) => _next = next;

        /// <summary>
        /// Invokes the middleware for each HTTP request.
        /// Catches any unhandled exception and converts it into a standardized JSON API response.
        /// </summary>
        /// <param name="httpContext">The current HTTP context.</param>
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                // Call the next middleware in the pipeline
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                // Handle any uncaught exception
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        /// <summary>
        /// Handles an exception by returning a standardized <see cref="ApiResponse{T}"/> JSON payload.
        /// </summary>
        /// <param name="context">The HTTP context to write the response.</param>
        /// <param name="ex">The exception to handle.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            // Set response content type to JSON
            context.Response.ContentType = "application/json";

            // Default to 500 Internal Server Error
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            // Create a standardized API error response
            var err = ApiResponse<object>.Fail(
                new ApiError(ex.Message, code: "INTERNAL_ERROR"), // Error details
                "Internal server error",                          // User-friendly message
                500                                               // Status code
            );

            // Serialize to JSON with camelCase naming
            var json = JsonSerializer.Serialize(
                err,
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
            );

            // Write JSON to the response body
            return context.Response.WriteAsync(json);
        }
    }
}