using System.Text.Json.Serialization;

namespace GenericApiResponse.Models
{
    /// <summary>
    /// Represents a single error in the API response.
    /// This class is used within <see cref="ApiResponse{T}"/> to provide detailed error information.
    /// </summary>
    public sealed class ApiError
    {
        /// <summary>
        /// A machine-readable error code that identifies the type of error.
        /// </summary>
        [JsonPropertyName("code")]
        public string? Code { get; init; }

        /// <summary>
        /// A human-readable error message describing what went wrong.
        /// </summary>
        [JsonPropertyName("message")]
        public string Message { get; init; }

        /// <summary>
        /// The field or property related to the error (useful for validation errors).
        /// </summary>
        [JsonPropertyName("field")]
        public string? Field { get; init; }

        /// <summary>
        /// Optional metadata providing additional context about the error.
        /// </summary>
        [JsonPropertyName("meta")]
        public IDictionary<string, object?>? Meta { get; init; }

        /// <summary>
        /// Initializes a new instance of <see cref="ApiError"/>.
        /// </summary>
        /// <param name="message">The human-readable error message. Cannot be null.</param>
        /// <param name="code">Optional machine-readable error code.</param>
        /// <param name="field">Optional field or property associated with the error.</param>
        /// <param name="meta">Optional metadata dictionary for additional context.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="message"/> is null.</exception>
        public ApiError(string message, string? code = null, string? field = null, IDictionary<string, object?>? meta = null)
        {
            Message = message ?? throw new ArgumentNullException(nameof(message));
            Code = code;
            Field = field;
            Meta = meta;
        }
    }
}