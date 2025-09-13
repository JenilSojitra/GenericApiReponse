using GenericApiResponse.Models;
using Microsoft.AspNetCore.Mvc;

namespace GenericApiResponse.Extensions
{
    /// <summary>
    /// Provides extension methods to convert generic API response models into standardized <see cref="ActionResult"/>.
    /// This ensures consistent HTTP status codes and response payloads across controllers.
    /// </summary>
    public static class ActionResultExtensions
    {
        /// <summary>
        /// Converts an <see cref="ApiResponse{T}"/> into an <see cref="ActionResult"/>.
        /// </summary>
        /// <typeparam name="T">The type of data returned in the API response.</typeparam>
        /// <param name="response">
        /// The generic API response that contains:
        /// <list type="bullet">
        ///   <item><description><c>Success</c>: Indicates if the operation succeeded.</description></item>
        ///   <item><description><c>Message</c>: Human-readable message describing the result.</description></item>
        ///   <item><description><c>Data</c>: Optional payload containing result data.</description></item>
        ///   <item><description><c>Errors</c>: Optional list of errors if the request failed.</description></item>
        ///   <item><description><c>Code</c>: Optional explicit HTTP status code override.</description></item>
        /// </list>
        /// </param>
        /// <returns>
        /// An <see cref="ActionResult"/> wrapping the <see cref="ApiResponse{T}"/> with an appropriate HTTP status code.
        /// <list type="bullet">
        ///   <item><description><c>200 OK</c>: If the response indicates success.</description></item>
        ///   <item><description><c>400 Bad Request</c>: If the response indicates failure and no explicit code is provided.</description></item>
        ///   <item><description><c>Custom Code</c>: If <c>response.Code</c> is explicitly set.</description></item>
        /// </list>
        /// </returns>
        public static ActionResult<ApiResponse<T>> ToActionResult<T>(this ApiResponse<T> response)
        {
            // If explicit code provided, use it; otherwise pick default mapping
            var code = response.Code ?? (response.Success ? 200 : 400);

            var actionResult = new ObjectResult(response) { StatusCode = code };
            return actionResult;
        }

        /// <summary>
        /// Converts a <see cref="PagedResponse{T}"/> into an <see cref="ActionResult"/>.
        /// </summary>
        /// <typeparam name="T">The type of data returned in the paged response.</typeparam>
        /// <param name="response">
        /// The paged API response that contains:
        /// <list type="bullet">
        ///   <item><description><c>Items</c>: The collection of items for the current page.</description></item>
        ///   <item><description><c>TotalCount</c>: The total number of items available across all pages.</description></item>
        ///   <item><description><c>PageNumber</c> and <c>PageSize</c>: Paging information.</description></item>
        ///   <item><description><c>Success</c>, <c>Message</c>, <c>Errors</c>: Same as <see cref="ApiResponse{T}"/>.</description></item>
        ///   <item><description><c>Code</c>: Optional explicit HTTP status code override.</description></item>
        /// </list>
        /// </param>
        /// <returns>
        /// An <see cref="ActionResult"/> wrapping the <see cref="PagedResponse{T}"/> with an appropriate HTTP status code.
        /// <list type="bullet">
        ///   <item><description><c>200 OK</c>: Default response if no explicit status code is provided.</description></item>
        ///   <item><description><c>Custom Code</c>: If <c>response.Code</c> is explicitly set.</description></item>
        /// </list>
        /// </returns>
        public static ActionResult<PagedResponse<T>> ToActionResult<T>(this PagedResponse<T> response)
        {
            var code = response.Code ?? 200;
            return new ObjectResult(response) { StatusCode = code };
        }
    }
}
