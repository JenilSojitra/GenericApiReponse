using System.Text.Json.Serialization;

namespace GenericApiResponse.Models
{
    /// <summary>
    /// Generic API response wrapper used for standardizing API responses.
    /// Provides information about success, message, data payload, errors, and metadata.
    /// </summary>
    /// <typeparam name="T">The type of the data payload returned by the API.</typeparam>
    public class ApiResponse<T>
    {
        /// <summary>
        /// Indicates whether the API request was successful.
        /// </summary>
        [JsonPropertyName("success")]
        public bool Success { get; init; }

        /// <summary>
        /// Optional human-readable message describing the response.
        /// Can be used for success, error, or informational messages.
        /// </summary>
        [JsonPropertyName("message")]
        public string? Message { get; init; }

        /// <summary>
        /// The data returned by the API. Null if the request failed or has no content.
        /// </summary>
        [JsonPropertyName("data")]
        public T? Data { get; init; }

        /// <summary>
        /// List of errors returned when the API request fails.
        /// </summary>
        [JsonPropertyName("errors")]
        public IReadOnlyList<ApiError>? Errors { get; init; }

        /// <summary>
        /// Optional dictionary for additional metadata (pagination info, totals, etc.).
        /// </summary>
        [JsonPropertyName("meta")]
        public IDictionary<string, object?>? Meta { get; init; }

        /// <summary>
        /// HTTP status code representing the API response.
        /// </summary>
        [JsonPropertyName("code")]
        public int? Code { get; init; }

        /// <summary>
        /// Protected constructor to allow derived types (like PagedResponse) to initialize the object.
        /// </summary>
        protected ApiResponse(
            bool success,
            T? data,
            string? message,
            IReadOnlyList<ApiError>? errors,
            IDictionary<string, object?>? meta,
            int? code)
        {
            Success = success;
            Data = data;
            Message = message;
            Errors = errors;
            Meta = meta;
            Code = code;
        }

        #region Factory Methods

        /// <summary>
        /// Creates a successful API response with optional data, message, metadata, and HTTP code.
        /// </summary>
        public static ApiResponse<T> Ok(
            T? data,
            string? message = null,
            IDictionary<string, object?>? meta = null,
            int? code = 200)
            => new ApiResponse<T>(true, data, message, null, meta, code);

        /// <summary>
        /// Creates a successful API response with no content (HTTP 204 by default).
        /// </summary>
        public static ApiResponse<T> NoContent(
            string? message = null,
            int? code = 204)
            => new ApiResponse<T>(true, default, message, null, null, code);

        /// <summary>
        /// Creates a failed API response from a list of errors with optional message and HTTP code.
        /// </summary>
        public static ApiResponse<T> Fail(
            IReadOnlyList<ApiError> errors,
            string? message = null,
            int? code = 400)
            => new ApiResponse<T>(false, default, message, errors, null, code);

        /// <summary>
        /// Creates a failed API response from a single error with optional message and HTTP code.
        /// </summary>
        public static ApiResponse<T> Fail(
            ApiError error,
            string? message = null,
            int? code = 400)
            => Fail(new[] { error }, message, code);

        #endregion
    }
}