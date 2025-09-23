# GenericApiResponse

<img width="735" height="587" alt="genericapiresponse" src="https://github.com/user-attachments/assets/373b3433-f804-4e5e-ab01-72e8a02a8a4b" />

**Author**: Jenil Sojitra  
**Company**: Chandra Solution  
**NuGet Package**: [GenericApiResponse](https://www.nuget.org/packages/GenericApiResponse)  
**Version**: 1.0.0  

## Overview

`GenericApiResponse` is a lightweight .NET library designed to standardize API responses in ASP.NET Core applications. It provides a consistent structure for success and error responses, including support for pagination, error handling, and integration with ASP.NET Core's middleware and filters. The library helps developers maintain clean, predictable, and maintainable API responses across their applications.

Key features include:
- **Standardized Response Format**: Wraps API responses in a consistent `ApiResponse<T>` structure with properties for success, data, errors, message, and metadata.
- **Pagination Support**: Includes `PagedResponse<T>` for handling paged data with strongly typed metadata.
- **Global Exception Handling**: Middleware to catch unhandled exceptions and return standardized error responses.
- **Result Filtering**: Automatically wraps controller responses in the standardized format using a result filter.
- **Problem Details Integration**: Converts ASP.NET Core `ProblemDetails` into the standardized response format.
- **Action Result Conversion**: Extension methods to convert responses into `ActionResult` with appropriate HTTP status codes.

## Installation

Install the `GenericApiResponse` package via NuGet Package Manager or the .NET CLI.

### Using NuGet Package Manager
```powershell
Install-Package GenericApiResponse -Version 1.0.0
```

### Using .NET CLI
```bash
dotnet add package GenericApiResponse --version 1.0.0
```

For projects using Central Package Management (CPM), add the following to your `Directory.Packages.props` file:
```xml
<PackageVersion Include="GenericApiResponse" Version="1.0.0" />
```

## Getting Started

To use `GenericApiResponse` in your ASP.NET Core application, follow these steps:

1. **Register the Exception Middleware**: Add the global exception handler to catch unhandled exceptions and return standardized responses.
2. **Register the Result Filter**: Add the result filter to automatically wrap controller responses.
3. **Use the Response Models**: Return `ApiResponse<T>` or `PagedResponse<T>` from your controllers.
4. **Handle Problem Details**: Convert validation or other errors into standardized responses using `ProblemDetailsMapper`.

### Setup in `Program.cs`

```csharp
using GenericApiResponse.Middleware;
using GenericApiResponse.Filters;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers(options =>
{
    // Register the result filter to wrap responses
    options.Filters.Add<AutoWrapResultFilter>();
});

// Add other services as needed
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Register the global exception handler
app.UseApiResponseExceptionHandler();

app.UseAuthorization();
app.MapControllers();

app.Run();
```

## Usage Examples

### 1. Returning a Successful Response
Use `ApiResponse<T>.Ok` to return a successful response with data.

```csharp
using GenericApiResponse.Models;
using GenericApiResponse.Extensions;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    [HttpGet("{id}")]
    public ActionResult GetUser(int id)
    {
        var user = new { Id = id, Name = "John Doe" };
        return ApiResponse<object>.Ok(user, "User retrieved successfully").ToActionResult();
    }
}
```

**Response (JSON)**:
```json
{
  "success": true,
  "message": "User retrieved successfully",
  "data": {
    "id": 1,
    "name": "John Doe"
  },
  "errors": null,
  "meta": null,
  "code": 200
}
```

### 2. Returning a Paged Response
Use `PagedResponse<T>.Create` to return paginated data.

```csharp
using GenericApiResponse.Models;
using GenericApiResponse.Extensions;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    [HttpGet]
    public ActionResult GetProducts(int page = 1, int pageSize = 10)
    {
        var products = new List<object>
        {
            new { Id = 1, Name = "Product A" },
            new { Id = 2, Name = "Product B" }
        };
        var totalItems = 25; // Example total count

        var response = PagedResponse<object>.Create(
            items: products,
            page: page,
            pageSize: pageSize,
            totalItems: totalItems,
            message: "Products retrieved successfully"
        );

        return response.ToActionResult();
    }
}
```

**Response (JSON)**:
```json
{
  "success": true,
  "message": "Products retrieved successfully",
  "data": [
    { "id": 1, "name": "Product A" },
    { "id": 2, "name": "Product B" }
  ],
  "errors": null,
  "meta": {
    "page": 1,
    "pageSize": 10,
    "totalItems": 25,
    "totalPages": 3
  },
  "code": 200
}
```

### 3. Handling Validation Errors
Use `ProblemDetailsMapper` to convert validation errors into a standardized response.

```csharp
using GenericApiResponse.Models;
using GenericApiResponse.Extensions;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    [HttpPost]
    public ActionResult CreateOrder([FromBody] OrderModel model)
    {
        if (!ModelState.IsValid)
        {
            var problem = new ProblemDetails
            {
                Title = "Validation Failed",
                Detail = "One or more fields are invalid.",
                Status = 400,
                Type = "validation_error"
            };
            return ProblemDetailsMapper.FromProblemDetails<object>(problem).ToActionResult();
        }

        // Process valid order
        var order = new { Id = 1, Item = model.Item };
        return ApiResponse<object>.Ok(order, "Order created successfully", code: 201).ToActionResult();
    }
}

public class OrderModel
{
    public string Item { get; set; }
}
```

**Error Response (JSON)**:
```json
{
  "success": false,
  "message": "Validation Failed",
  "data": null,
  "errors": [
    {
      "code": "validation_error",
      "message": "One or more fields are invalid.",
      "field": null,
      "meta": null
    }
  ],
  "meta": null,
  "code": 400
}
```

### 4. Global Exception Handling
The `ApiResponseExceptionMiddleware` automatically catches unhandled exceptions and returns a standardized error response.

**Example Exception**:
If an unhandled exception occurs (e.g., database connection failure), the middleware will catch it and return:

```json
{
  "success": false,
  "message": "Internal server error",
  "data": null,
  "errors": [
    {
      "code": "INTERNAL_ERROR",
      "message": "Database connection failed",
      "field": null,
      "meta": null
    }
  ],
  "meta": null,
  "code": 500
}
```

### 5. Automatic Response Wrapping
The `AutoWrapResultFilter` ensures that all controller responses are wrapped in `ApiResponse<T>` unless explicitly returned as `ApiResponse<T>` or `PagedResponse<T>`.

```csharp
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ValuesController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        var values = new[] { "value1", "value2" };
        return Ok(values); // Will be automatically wrapped
    }
}
```

**Response (JSON)**:
```json
{
  "success": true,
  "message": null,
  "data": ["value1", "value2"],
  "errors": null,
  "meta": null,
  "code": 200
}
```

## Configuration

- **JSON Serialization**: The library uses `System.Text.Json` with camelCase naming policy for JSON serialization. Ensure your ASP.NET Core application is configured to use camelCase if you customize JSON serialization globally.
- **Middleware Order**: Place `app.UseApiResponseExceptionHandler()` before `app.UseAuthorization()` and `app.MapControllers()` in `Program.cs` to ensure exceptions are caught early.
- **Filter Registration**: The `AutoWrapResultFilter` is registered globally in the example above. You can also apply it to specific controllers or actions using `[ServiceFilter(typeof(AutoWrapResultFilter))]`.

## Requirements

- **.NET Version**: .NET 6.0 or later
- **Dependencies**:
  - Microsoft.AspNetCore.Mvc.Core
  - System.Text.Json

## Contributing

Contributions are welcome! Please submit issues or pull requests to the [GitHub repository](https://github.com/JenilSojitra/GenericApiReponse). Ensure code follows the existing style and includes tests.

## Contact

For questions or support, contact Jenil Sojitra at Chandra Solution via [jenilsojitra19@gmail.com](mailto:jenilsojitra19@gmail.com) or open an issue on the GitHub repository.
