using Microsoft.AspNetCore.Builder;

namespace GenericApiResponse.Middleware
{
    /// <summary>
    /// Provides extension methods to easily register the <see cref="ApiResponseExceptionMiddleware"/> in the ASP.NET Core pipeline.
    /// </summary>
    public static class ExceptionMiddlewareExtensions
    {
        /// <summary>
        /// Adds the <see cref="ApiResponseExceptionMiddleware"/> to the application's middleware pipeline.
        /// This ensures all unhandled exceptions are caught and returned as standardized <see cref="GenericApiResponse.Models.ApiResponse{T}"/> JSON responses.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/> instance to configure the middleware on.</param>
        /// <returns>The <see cref="IApplicationBuilder"/> with the middleware registered.</returns>
        /// <example>
        /// Usage in <c>Program.cs</c>:
        /// <code>
        /// var builder = WebApplication.CreateBuilder(args);
        /// var app = builder.Build();
        /// 
        /// // Register global exception handler
        /// app.UseApiResponseExceptionHandler();
        /// 
        /// app.MapControllers();
        /// app.Run();
        /// </code>
        /// </example>
        public static IApplicationBuilder UseApiResponseExceptionHandler(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ApiResponseExceptionMiddleware>();
        }
    }
}
