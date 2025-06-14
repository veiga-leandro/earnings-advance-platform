using Asp.Versioning;
using Earnings.Advance.Platform.Application.Interfaces;
using Earnings.Advance.Platform.Application.Services;
using Earnings.Advance.Platform.Domain.Interfaces;
using Earnings.Advance.Platform.Infrastructure.Data;
using Earnings.Advance.Platform.Infrastructure.Repositories;
using Earnings.Advance.Platform.WebApi.Utils;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers(options =>
{
    // Configure route token replacements to use lowercase
    options.Conventions.Add(new RouteTokenTransformerConvention(
        new SlugifyParameterTransformer()));
});

// API Versioning Configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddApiVersioning(options =>
{
    // Default version when not specified
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;

    // How version will be specified in the request
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),           // /api/v1/advances
        new QueryStringApiVersionReader("version"), // ?version=1.0
        new HeaderApiVersionReader("X-Version"),    // Header: X-Version: 1.0
        new MediaTypeApiVersionReader("ver")        // Accept: application/json;ver=1.0
    );

    // How to report supported versions
    options.ApiVersionSelector = new CurrentImplementationApiVersionSelector(options);
}).AddApiExplorer(setup =>
{
    setup.GroupNameFormat = "'v'VVV";
    setup.SubstituteApiVersionInUrl = true;
});

// Entity Framework
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("AdvanceDb"));

// Dependency Injection
builder.Services.AddScoped<IAdvanceRepository, AdvanceRepository>();
builder.Services.AddScoped<IAdvanceService, AdvanceService>();

// OpenAPI Configuration
builder.Services.AddSwaggerGen(options =>
{
    // Define only V1 documentation
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Earnings Advance API",
        Version = "v1",
        Description = """
            API for managing earnings advance requests.
            
            Features:
            - Create advance requests
            - List requests by creator
            - Approve/Reject requests
            - Simulate advance fees
            
            All monetary values are in R$ (BRL) with 2 decimal places.
            """,
        Contact = new OpenApiContact
        {
            Name = "Development Team"
        },
        License = new OpenApiLicense
        {
            Name = "MIT License",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });

    // FILTER: Include ONLY version 1.0 endpoints
    options.DocInclusionPredicate((docName, apiDesc) =>
    {
        // Only process if it's for v1 documentation
        if (docName != "v1") return false;

        // Check if endpoint has version attribute
        var apiVersions = apiDesc.ActionDescriptor.EndpointMetadata
            .OfType<ApiVersionAttribute>()
            .SelectMany(attr => attr.Versions);

        // Include only if it contains version 1.0
        return apiVersions.Any(v => v.ToString() == "1.0");
    });

    // Include XML comments
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }

    // Additional settings
    options.EnableAnnotations();
    options.UseInlineDefinitionsForEnums();
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddJsonConsole(options =>
{
    options.IncludeScopes = false;
    options.TimestampFormat = "yyyy-MM-dd HH:mm:ss.fff";
    options.JsonWriterOptions = new JsonWriterOptions
    {
        Indented = true
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        // Configure only V1 endpoint
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Earnings Advance API v1");

        // UI Settings
        options.RoutePrefix = "swagger"; // URL: /swagger
        options.DocumentTitle = "Earnings Advance API - Documentation";
        options.DefaultModelsExpandDepth(2);
        options.DefaultModelRendering(Swashbuckle.AspNetCore.SwaggerUI.ModelRendering.Example);
        options.DisplayRequestDuration();
        options.EnableDeepLinking();
        options.EnableFilter();
        options.ShowExtensions();
    });
}

app.UseHttpsRedirection();
app.UseCors();

// Global exception handling
app.UseExceptionHandler("/error");

app.UseAuthorization();
app.MapControllers();

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureCreated();
}

app.Run();
