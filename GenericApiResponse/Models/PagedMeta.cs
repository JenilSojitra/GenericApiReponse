using System.Text.Json.Serialization;

namespace GenericApiResponse.Models
{
    /// <summary>
    /// Represents pagination metadata for a paged API response.
    /// Provides information about the current page, page size, total items, and total pages.
    /// </summary>
    public sealed class PagedMeta
    {
        /// <summary>
        /// The current page number (1-based index).
        /// </summary>
        [JsonPropertyName("page")]
        public int Page { get; init; }

        /// <summary>
        /// The number of items per page.
        /// </summary>
        [JsonPropertyName("pageSize")]
        public int PageSize { get; init; }

        /// <summary>
        /// The total number of items available across all pages.
        /// </summary>
        [JsonPropertyName("totalItems")]
        public long TotalItems { get; init; }

        /// <summary>
        /// The total number of pages available.
        /// </summary>
        [JsonPropertyName("totalPages")]
        public int TotalPages { get; init; }
    }
}