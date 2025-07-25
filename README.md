# ShopDataSampleAPI

A sample .NET 8 REST API that demonstrates basic API functionality including a nesting data sample endpoint.

## Overview

ShopDataSampleAPI is a lightweight ASP.NET Core API that demonstrates how to create a simple REST API with a POST endpoint that processes and returns JSON data. The API includes:

- A `NestingSample` controller with a `NEST` endpoint
- Model classes for request and response objects
- Configuration for VS Code debugging and running

## Project Structure

- `Controllers/`: Contains API controllers
- `Models/`: Contains request and response data models
- `.vscode/`: Contains VS Code configuration for debugging and tasks

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- Visual Studio Code with C# extension (recommended)

### Running the API

#### Using Visual Studio Code

1. Open the project in Visual Studio Code
2. Press `F5` or use the "Run and Debug" sidebar and select "Start API"
3. The API will start and listen on `http://localhost:5165` by default

#### Using Terminal

1. Navigate to the project directory
```bash
cd /path/to/ShopDataSampleAPI
```

2. Run the API
```bash
dotnet run
```

3. The terminal will display the URL where the API is listening (typically `http://localhost:5165`)

## Testing the API

### Using cURL

You can test the API using cURL from your terminal:

```bash
curl -X POST http://localhost:5165/api/NestingSample/NEST \
  -H "Content-Type: application/json" \
  -d '{"input":"test data"}'
```

Expected response:
```json
{
  "input": "test data",
  "output": "Hello world"
}
```

### Using AI Assistant

You can ask the AI Assistant to test the endpoint with a command like:

"Please run a curl command to test the NestingSample controller's NEST endpoint with some input data."

## Understanding .NET REST APIs

### Key Components

1. **Controllers**: Classes that handle HTTP requests and return responses
   - Located in the `Controllers/` directory
   - Inherit from `ControllerBase`
   - Decorated with `[ApiController]` and `[Route("api/[controller]")]` attributes

2. **Models**: Classes that define the structure of request and response data
   - Located in the `Models/` directory
   - Used for data transfer between client and server

3. **Program.cs**: The entry point for the application
   - Configures services and middleware
   - Sets up routing and dependency injection

### Common HTTP Methods

- **GET**: Retrieve data
- **POST**: Create new data or process information
- **PUT**: Update existing data
- **DELETE**: Remove data

### Content Negotiation

ASP.NET Core supports multiple formats for request and response data:
- JSON (default)
- XML
- Custom formats via formatters

### Dependency Injection

ASP.NET Core has built-in dependency injection which allows:
- Loose coupling between services
- Easy testing
- Lifecycle management of services

## Next Steps

- Add authentication and authorization
- Implement additional CRUD operations
- Add database integration with Entity Framework Core
- Implement logging and monitoring

## Notes on Nesting Implementation

This is where the nesting code from C would be called, and then the results would be returned as a valid JSON response. The implementation would process the input data and generate nested output data.

## Calling C++ DLLs from the Controller

### Overview
ASP.NET Core applications can call native C++ DLLs using Platform Invocation Services (P/Invoke) or the newer approach of using `NativeAOT` with C# interoperability. Below is a guide on how to integrate a C++ DLL with the NestingSample controller.

### Steps to Integrate a C++ DLL

#### 1. Create a wrapper class for P/Invoke

Create a new folder called `NativeInterop` and add a class to handle DLL imports:

```csharp
using System.Runtime.InteropServices;

namespace ShopDataSampleAPI.NativeInterop
{
    public static class NestingNative
    {
        // Define the path to your DLL
        private const string DllPath = "NestingLib";

        // Define the function signature from your C++ DLL
        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ProcessNesting(
            [MarshalAs(UnmanagedType.LPStr)] string input, 
            [MarshalAs(UnmanagedType.LPStr)] StringBuilder output, 
            int outputSize);

        // Wrapper method for easier use
        public static string Nest(string input)
        {
            // Allocate buffer for the C++ function output
            StringBuilder output = new StringBuilder(1024);
            int result = ProcessNesting(input, output, output.Capacity);
            
            if (result < 0)
            {
                throw new Exception($"Nesting processing failed with error code {result}");
            }
            
            return output.ToString();
        }
    }
}
```

#### 2. Place the C++ DLL in the correct location

- For development: Place the DLL in the project's output directory (`bin/Debug/net8.0/`)
- For production: Include the DLL in your deployment package

#### 3. Modify the controller to use the native DLL

Update the `NestingSampleController.cs` to use the wrapper class:

```csharp
using Microsoft.AspNetCore.Mvc;
using ShopDataSampleAPI.Models;
using ShopDataSampleAPI.NativeInterop;
using System.Text.Json;

namespace ShopDataSampleAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NestingSampleController : ControllerBase
    {
        [HttpPost("NEST")]
        public IActionResult Nest([FromBody] NestingRequest request)
        {
            try
            {
                // Call the C++ DLL through our wrapper
                string nestedResult = NestingNative.Nest(request.Input);
                
                var response = new NestingResponse
                {
                    Input = request.Input,
                    Output = nestedResult
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error processing nesting: {ex.Message}");
            }
        }
    }
}
```

#### 4. Build considerations for cross-platform deployment

- **Windows**: Use `.dll` files
- **macOS**: Use `.dylib` files
- **Linux**: Use `.so` files

You may need conditional loading based on the operating system:

```csharp
private const string DllPath = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) 
    ? "NestingLib.dll" 
    : RuntimeInformation.IsOSPlatform(OSPlatform.OSX) 
        ? "libNestingLib.dylib" 
        : "libNestingLib.so";
```

### Performance Considerations

- **Marshaling Overhead**: Converting between managed and unmanaged data types adds overhead
- **Memory Management**: Be careful with native memory allocations and ensure proper cleanup
- **Error Handling**: Robust error handling for C++ exceptions and error codes is essential
- **Threading**: Consider thread safety when calling native code from multiple requests

### Security Considerations

- Validate all inputs before passing to the C++ DLL to prevent buffer overflows
- Ensure the DLL comes from a trusted source
- Follow the principle of least privilege when granting access to the DLL

---

Created: July 24, 2025
