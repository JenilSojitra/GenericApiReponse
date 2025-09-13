using System.Text.Json.Serialization;

namespace GenericApiResponse.Models
{
    /// <summary>
    /// Represents a paged API response with a strongly typed <see cref="PagedMeta"/> object.
    /// Inherits from <see cref="ApiResponse{T}"/> with <see cref="IReadOnlyList{T}"/> as data.
    /// </summary>
    /// <typeparam name="T">The type of items in the paged response.</typeparam>
    public sealed class PagedResponse<T> : ApiResponse<IReadOnlyList<T>>
    {
        /// <summary>
        /// Strongly typed pagination metadata for client convenience.
        /// </summary>
        [JsonPropertyName("meta")]
        public new PagedMeta Meta { get; init; }

        /// <summary>
        /// Private constructor that initializes the strongly typed <see cref="Meta"/> 
        /// and also populates the base <c>Meta</c> dictionary for backward compatibility.
        /// </summary>
        /// <param name="data">The items in the current page.</param>
        /// <param name="meta">Strongly typed pagination metadata.</param>
        /// <param name="message">Optional message for the response.</param>
        /// <param name="code">Optional HTTP status code (default 200).</param>
        private PagedResponse(IReadOnlyList<T> data, PagedMeta meta, string? message, int? code)
            : base(
                  true,
                  data,
                  message,
                  null,
                  new Dictionary<string, object?>
                  {
                      ["page"] = meta.Page,
                      ["pageSize"] = meta.PageSize,
                      ["totalItems"] = meta.TotalItems,
                      ["totalPages"] = meta.TotalPages
                  },
                  code)
        {
            Meta = meta;
        }

        /// <summary>
        /// Factory method to create a paged response from any enumerable collection.
        /// </summary>
        /// <param name="items">The items in the current page.</param>
        /// <param name="page">The current page number (1-based).</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <param name="totalItems">The total number of items across all pages.</param>
        /// <param name="message">Optional response message.</param>
        /// <param name="code">Optional HTTP status code (default 200).</param>
        /// <returns>A <see cref="PagedResponse{T}"/> containing the paged items and metadata.</returns>
        public static PagedResponse<T> Create(
            IEnumerable<T>? items,
            int page,
            int pageSize,
            long totalItems,
            string? message = null,
            int? code = 200)
        {
            var list = items as IReadOnlyList<T> ?? items?.ToList() ?? new List<T>();

            // Ensure page and pageSize are valid
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = list.Count == 0 ? 1 : pageSize;

            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            var meta = new PagedMeta
            {
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages
            };

            return new PagedResponse<T>(list, meta, message, code);
        }
    }
}