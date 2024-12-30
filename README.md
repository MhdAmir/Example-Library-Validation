
# Sindika.AspNet.Enrichment Library

The **Sindika.AspNet.Enrichment** library provides a middleware-based solution for enriching HTTP requests with custom data. This guide explains how to install, configure, and use the library to integrate enrichment functionality into your .NET applications.

---

## Prerequisites

1. **.NET 9 SDK**
   - Download and install the .NET SDK from [Microsoft's official site](https://dotnet.microsoft.com/download).

2. **NuGet Package Manager**
   - Ensure you have access to NuGet for package installation.

3. **Basic Knowledge**
   - Familiarity with ASP.NET Core middleware and dependency injection.

---

## Installation

### 1. Add the NuGet Package

Run the following command in your terminal or Package Manager Console to install the library:

```bash
dotnet add package Sindika.AspNet.Validation
```

Alternatively, search for `Sindika.AspNet.Enrichment` in the NuGet Package Manager within your IDE and install it.

---


## Available Enrichers

Here is a list of enrichers provided by the **Sindika.AspNet.Validation** library:

### Enricher Classes Overview

This table provides an overview of the available enricher classes in the **Sindika.AspNet.Enrichment** library, including their corresponding `Name` property and description.

| **No.** | **Class Name**               | **Name**           | **Description**                                                                 |
|---------|------------------------------|--------------------|---------------------------------------------------------------------------------|
| **1**   | `RequestIdEnricher`          | `RequestId`        | Generates and attaches a unique request identifier.                             |
| **2**   | `CorrelationIdEnricher`      | `CorrelationId`    | Adds correlation IDs to link requests across distributed systems.               |
| **3**   | `RequestUrlEnricher`         | `RequestUrl`       | Extracts and appends the full request URL.                                      |
| **4**   | `ClientIpEnricher`           | `ClientIp`         | Extracts and appends the client's IP address.                                   |
| **5**   | `RequestTimeEnricher`        | `RequestTime`      | Records and enriches the request start time.                                    |
| **6**   | `ClientInfoEnricher`         | `ClientInfo`       | Extracts client information such as browser, OS, and device from User-Agent.    |
| **7**   | `UserInformationEnricher`    | `UserInformation`  | Extracts user context and claims from JWTs or tokens.                           |
| **8**   | `MachineInformationEnricher`| `MachineInformation`| Adds machine-specific data such as machine ID and thread ID.                    |
| **9**   | `HttpHeaderEnricher`         | `HttpHeader`       | Extracts and appends useful metadata from HTTP headers.                         |
| **10**  | `QueryParameterEnricher`     | `QueryParameter`   | Extracts query parameters from both GET and POST requests.                      |
| **11**  | `BatchEnricher`              | `BatchEnricher`    | Processes multiple enrichers in a single batch.                                 |


## Configuration

### 1. Add `EnrichmentSettings` to `appsettings.json`

Add the following configuration section to your `appsettings.json` file:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "EnrichmentSettings": {
    "EnabledEnrichers": [
      "RequestId",
      "CorrelationId",
      "RequestUrl",
      "ClientIp",
      "RequestTime",
      "ClientInfo",
      "UserInformation",
      "MachineInformation",
      "HttpHeader",
      "QueryParameter"
    ],
    "MaxRetries": 3,
    "RetryBaseDelayMilliseconds": 100,
    "EnableDebugLogging": true,
    "BatchProcessing": {
      "MaxBatchSize": 15,
      "MaxQueueSize": 200,
      "MaxConcurrentBatches": 5,
      "BatchIntervalMilliseconds": 100
    }
  }
}

```

### 2. Configure Services in `Program.cs`

In your `Program.cs` file, register the enrichment services and settings:

```csharp
using Sindika.AspNet.Enrichment;
using Sindika.AspNet.Enrichment.Configuration;
using Sindika.AspNet.Enrichment.Enrichment.Middleware;


var builder = WebApplication.CreateBuilder(args);

// Add open API
builder.Services.AddOpenApi();

// Configure EnrichmentSettings from appsettings.json
builder.Services.Configure<EnrichmentSettings>(builder.Configuration.GetSection("EnrichmentSettings"));

// Register enrichment services
builder.Services.AddEnrichment();

var app = builder.Build();
app.UseMiddleware<EnrichmentMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/weatherforecast", async (HttpContext context) =>
{
    var forecast = new[]
    {
        new { Date = DateTime.Now, TemperatureC = 20, Summary = "Cool" }
    };

    var enrichedData = context.Items["EnrichedData"] as Dictionary<string, object>;

    return await Task.FromResult(new { Forecast = forecast, EnrichedData = enrichedData });
});

app.Run();

```

---

### Example Enriched Data

When the middleware processes a request, it enriches the data and makes it available in `HttpContext.Items["EnrichedData"]`. Here’s an example response:

```json
{
    "weatherForecast": [
        {
            "date": "2024-12-26",
            "temperatureC": 29,
            "summary": "Hot",
            "temperatureF": 84
        }
    ],
    "enrichedData": {
        "UserInfo": {
            "IsAuthenticated": false,
            "UserName": "Anonymous",
            "Claims": {}
        },
        "ClientInfo": {
            "OperatingSystem": "Windows 10",
            "Browser": "Chrome 131.0",
            "Device": "Other",
            "UserAgent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/131.0.0.0 Safari/537.36"
        },
        "CorrelationId": "b216b679-4749-495f-89d9-99ee1303a9c8",
        "RequestStartTime": "2024-12-25T02:31:36.606202Z",
        "RequestId": "01d7b245-fccd-4739-be82-e84b9fc660d6",
        "QueryParams": {
            "a": "2"
        },
        "HttpHeaders": {
            "Accept": "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7",
            "Connection": "keep-alive",
            "Host": "localhost:5206",
            "User-Agent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/131.0.0.0 Safari/537.36",
            "Accept-Encoding": "gzip, deflate, br, zstd",
            "Accept-Language": "en-US,en;q=0.9,ko;q=0.8",
            "Upgrade-Insecure-Requests": "1",
            "sec-ch-ua": "\"Google Chrome\";v=\"131\", \"Chromium\";v=\"131\", \"Not_A Brand\";v=\"24\"",
            "sec-ch-ua-mobile": "?0",
            "sec-ch-ua-platform": "\"Windows\"",
            "Sec-Fetch-Site": "none",
            "Sec-Fetch-Mode": "navigate",
            "Sec-Fetch-User": "?1",
            "Sec-Fetch-Dest": "document",
            "x-token": "628074a89784267a95d5569ef909db8c4280ebc873cfc3c28456da6f1b7860cb"
        },
        "RequestUrl": "http://localhost:5206/weatherforecast?a=2",
        "ClientIp": "::1",
        "MachineInfo": {
            "MachineName": "DESKTOP-OBNDKRQ",
            "OSVersion": "Microsoft Windows NT 10.0.22631.0",
            "ProcessorCount": 20,
            "Is64BitProcess": true,
            "DotNetVersion": "9.0.0",
            "ThreadId": 11,
            "TotalMemory": 10893520
        }
    }
}
```


## Manually Register

### Step 1: Configure Services in `Program.cs`

Manually register `enrichers` and the `EnrichmentManager` in the DI container:

```csharp
var builder = WebApplication.CreateBuilder(args);

// Register enrichers
builder.Services.AddSingleton<IEnricher, RequestIdEnricher>();
builder.Services.AddSingleton<IEnricher, ClientIpEnricher>();

// Register the enrichment manager
builder.Services.AddScoped<EnrichmentManager>();

```

### Step 2: Add Enrichment Middleware

Use middleware to execute the enrichment pipeline:

```csharp
app.Use(async (context, next) =>
{
    var enrichmentManager = context.RequestServices.GetRequiredService<EnrichmentManager>();

    // Execute enrichment
    var enrichedData = await enrichmentManager.EnrichAsync(context);

    // Attach enriched data to HttpContext
    context.Items["EnrichedData"] = enrichedData;

    await next.Invoke();
});
```

### Step 3: Access Enriched Data

Create an endpoint to demonstrate enriched data usage:

```csharp
app.MapGet("/enriched-data", (HttpContext context) =>
{
    var enrichedData = context.Items["EnrichedData"] as Dictionary<string, object>;
    return enrichedData ?? new Dictionary<string, object>();
});
```


## Notes

- Each enricher class implements the `IEnricher` interface.
- The `Name` property is used in configurations to enable or disable specific enrichers.
- Descriptions provide a high-level overview of the functionality of each enricher.


---

## Customization

### Adding Custom Enrichers

1. **Create a New Enricher**

Implement the `IEnricher` interface to define your custom enrichment logic:

```csharp
public class CustomEnricher : IEnricher
{
    public string Name => "CustomEnricher";
    public int Priority => 1;
    public bool IsCritical => false;

    public async Task<Dictionary<string, object>> EnrichAsync(HttpContext context)
    {
        return await Task.FromResult(new Dictionary<string, object>
        {
            { "CustomData", "YourValue" }
        });
    }
}
```

2. **Register the Enricher**

Add the enricher to your `AddEnrichment` call or manually in `Program.cs`:

```csharp
builder.Services.AddSingleton<IEnricher, CustomEnricher>();
```

---

## Testing

### Unit Testing

1. **Test Enrichment Logic**

Write tests for individual enrichers:

```csharp
[Fact]
public async Task CustomEnricher_ShouldReturnCustomData()
{
    var context = new DefaultHttpContext();
    var enricher = new CustomEnricher();

    var result = await enricher.EnrichAsync(context);

    Assert.NotNull(result);
    Assert.Equal("YourValue", result["CustomData"]);
}
```

2. **Test the Middleware**

Verify the middleware processes enrichment correctly:

```csharp
[Fact]
public async Task Middleware_ShouldAttachEnrichedDataToContext()
{
    var services = new ServiceCollection();
    services.AddEnrichment();
    var serviceProvider = services.BuildServiceProvider();

    var context = new DefaultHttpContext();
    context.RequestServices = serviceProvider;

    var middleware = new EnrichmentMiddleware(async _ => { });

    await middleware.InvokeAsync(context);

    Assert.True(context.Items.ContainsKey("EnrichedData"));
}
```
