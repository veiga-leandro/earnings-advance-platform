using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;
using System.Net.Http.Json;
using System.Text.Json;

namespace Earnings.Advance.Platform.IntegrationTests.TestBase
{
    public abstract class IntegrationTestBase : IClassFixture<WebApplicationFactory<Program>>
    {
        protected readonly WebApplicationFactory<Program> _factory;
        protected readonly HttpClient _client;
        protected readonly JsonSerializerOptions _jsonOptions;

        protected IntegrationTestBase(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Testing");
            });

            _client = _factory.CreateClient();
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        protected async Task<T?> DeserializeResponse<T>(HttpResponseMessage response)
        {
            return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
        }
    }
}