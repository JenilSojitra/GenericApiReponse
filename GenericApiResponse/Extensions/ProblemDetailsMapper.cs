using GenericApiResponse.Models;
using Microsoft.AspNetCore.Mvc;

namespace GenericApiResponse.Extensions
{
    /// <summary>
    /// Provides mapping utilities to convert <see cref="ProblemDetails"/> into a standardized <see cref="ApiResponse{T}"/>.
    /// Ensures consistent error responses across the API.
    /// </summary>
    public static class ProblemDetailsMapper
    {
        /// <summary>
        /// Converts a <see cref="ProblemDetails"/> instance into a failed <see cref="ApiResponse{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of data expected in the response. Typically unused in error cases.</typeparam>
        /// <param name="pd">
        /// The <see cref="ProblemDetails"/> object provided by ASP.NET Core that contains:
        /// <list type="bullet">
        ///   <item><description><c>Title</c>: A short, human-readable summary of the error.</description></item>
        ///   <item><description><c>Detail</c>: A detailed description of the error (if available).</description></item>
        ///   <item><description><c>Status</c>: The associated HTTP status code (e.g., 400, 404, 500).</description></item>
        ///   <item><description><c>Type</c>: A URI reference that identifies the problem type (used as error code).</description></item>
        /// </list>
        /// </param>
        /// <returns>
        /// A failed <see cref="ApiResponse{T}"/> containing:
        /// <list type="bullet">
        ///   <item><description><c>Success</c>: Always false.</description></item>
        ///   <item><description><c>Message</c>: Taken from <c>Title</c>.</description></item>
        ///   <item><description><c>Errors</c>: A single <see cref="ApiError"/> with <c>Detail</c> or <c>Title</c> as the message, and <c>Type</c> as the error code.</description></item>
        ///   <item><description><c>Code</c>: Populated from <c>Status</c>.</description></item>
        /// </list>
        /// </returns>
        /// <example>
        /// Example usage in a controller:
        /// <code>
        /// var problem = new ProblemDetails
        /// {
        ///     Title = "Validation Failed",
        ///     Detail = "One or more fields are invalid.",
        ///     Status = 400,
        ///     Type = "validation_error"
        /// };
        ///
        /// var response = ProblemDetailsMapper.FromProblemDetails&lt;object&gt;(problem);
        /// return response.ToActionResult();
        /// </code>
        /// </example>
        public static ApiResponse<T> FromProblemDetails<T>(ProblemDetails pd)
        {
            var error = new ApiError(pd.Detail ?? pd.Title ?? "Error", code: pd.Type);
            return ApiResponse<T>.Fail(error, pd.Title, pd.Status);
        }
    }
}
