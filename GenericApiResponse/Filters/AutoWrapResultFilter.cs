using GenericApiResponse.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GenericApiResponse.Filters
{
    /// <summary>
    /// A result filter that automatically wraps API responses into a standardized <see cref="ApiResponse{T}"/>.
    /// Ensures consistency in success and error payloads across the application.
    /// </summary>
    /// <remarks>
    /// This filter:
    /// <list type="bullet">
    ///   <item><description>Skips wrapping if the response is already an <see cref="ApiResponse{T}"/>.</description></item>
    ///   <item><description>Converts <see cref="NoContentResult"/> into a <c>NoContent</c> <see cref="ApiResponse{T}"/>.</description></item>
    ///   <item><description>Wraps any <see cref="ObjectResult"/> (e.g., controller returns data) into <c>ApiResponse.Ok</c>.</description></item>
    /// </list>
    /// </remarks>
    public class AutoWrapResultFilter : IAsyncResultFilter
    {
        /// <summary>
        /// Intercepts the action result before execution and wraps it into <see cref="ApiResponse{T}"/>.
        /// </summary>
        /// <param name="context">The current result execution context.</param>
        /// <param name="next">The delegate to execute the next filter or action result.</param>
        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            // If already an ApiResponse, skip wrapping
            if (context.Result is ObjectResult o
                && o.Value != null
                && o.Value.GetType().IsGenericType
                && o.Value.GetType().GetGenericTypeDefinition() == typeof(ApiResponse<>))
            {
                await next();
                return;
            }

            // If result is NoContent (HTTP 204)
            if (context.Result is NoContentResult)
            {
                var wrapped = ApiResponse<object>.NoContent();
                context.Result = new ObjectResult(wrapped) { StatusCode = 204 };
                await next();
                return;
            }

            // Wrap ObjectResult (any returned object) in a success response
            if (context.Result is ObjectResult objectResult)
            {
                var wrapped = ApiResponse<object>.Ok(
                    objectResult.Value,        // Original response value
                    message: null,             // Optional message (unused here)
                    code: objectResult.StatusCode // Preserve status code if set
                );

                context.Result = new ObjectResult(wrapped)
                {
                    StatusCode = objectResult.StatusCode ?? 200
                };
            }

            await next();
        }
    }
}