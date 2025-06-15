using Earnings.Advance.Platform.Application.DTOs.Advance;
using Earnings.Advance.Platform.IntegrationTests.TestBase;
using NBomber.CSharp;
using System.Net.Http.Json;
using System.Text.Json;

namespace Earnings.Advance.Platform.IntegrationTests.LoadTests
{
    public class AdvanceLoadTests : IntegrationTestBase, IClassFixture<CustomWebApplicationFactory>
    {
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public AdvanceLoadTests(CustomWebApplicationFactory factory) : base(factory)
        {
        }

        [Fact]
        public void LoadTest_SimulateAdvance()
        {
            const int RatePerSecond = 100;
            const int DurationSeconds = 30;

            var httpClient = _factory.CreateClient();

            var scenario = Scenario.Create("simulate_advance", async context =>
            {
                var amount = Random.Shared.Next(101, 10000);
                var request = new HttpRequestMessage(HttpMethod.Get,
                    $"/api/v1/advance/simulate?amount={amount}");

                var response = await httpClient.SendAsync(request);
                return Response.Ok(statusCode: response.StatusCode.ToString());
            })
            .WithLoadSimulations(
                Simulation.Inject(rate: RatePerSecond,
                                interval: TimeSpan.FromSeconds(1),
                                during: TimeSpan.FromSeconds(DurationSeconds))
            );

            var stats = NBomberRunner
                .RegisterScenarios(scenario)
                .WithTestName("Advance Simulation Load Test")
                .WithTestSuite("API Load Tests")
                .Run();

            // Validations
            Assert.True(stats.AllOkCount > 0);
            Assert.True(stats.AllFailCount == 0, $"Found {stats.AllFailCount} failed requests");
            Assert.True(stats.ScenarioStats[0].Ok.Request.RPS >= RatePerSecond * 0.9);
        }

        [Fact]
        public void LoadTest_CreateAdvanceRequest()
        {
            const int RatePerSecond = 50;
            const int DurationSeconds = 30;

            var httpClient = _factory.CreateClient();

            var scenario = Scenario.Create("create_advance", async context =>
            {
                var request = new CreateAdvanceRequestDto
                {
                    CreatorId = Guid.NewGuid(),
                    RequestedAmount = Random.Shared.Next(101, 10000)
                };

                var response = await httpClient.PostAsJsonAsync("/api/v1/advance", request);
                return Response.Ok(statusCode: response.StatusCode.ToString());
            })
            .WithLoadSimulations(
                Simulation.Inject(rate: RatePerSecond,
                                interval: TimeSpan.FromSeconds(1),
                                during: TimeSpan.FromSeconds(DurationSeconds))
            );

            var stats = NBomberRunner
                .RegisterScenarios(scenario)
                .WithTestName("Advance Creation Load Test")
                .WithTestSuite("API Load Tests")
                .Run();

            // Validations
            Assert.True(stats.AllOkCount > 0);
            Assert.True(stats.AllFailCount == 0, $"Found {stats.AllFailCount} failed requests");
            Assert.True(stats.ScenarioStats[0].Ok.Request.RPS >= RatePerSecond * 0.9);
        }

        [Fact]
        public void LoadTest_ListAdvanceRequests()
        {
            const int RatePerSecond = 200;
            const int DurationSeconds = 30;

            var httpClient = _factory.CreateClient();
            var creatorId = Guid.NewGuid();

            // Create test data - Create and approve requests
            for (int i = 0; i < 10; i++)
            {
                // Create request
                var request = new CreateAdvanceRequestDto
                {
                    CreatorId = creatorId,
                    RequestedAmount = 1000m + (i * 100)
                };
                var createResponse = httpClient.PostAsJsonAsync("/api/v1/advance", request).Result;
                createResponse.EnsureSuccessStatusCode();

                // Get the created request ID
                var createdAdvance = createResponse.Content
                    .ReadFromJsonAsync<AdvanceRequestResponseDto>().Result;

                // Approve the request to allow creating new ones
                var approveResponse = httpClient
                    .PatchAsync($"/api/v1/advance/{createdAdvance!.Id}/approve", null)
                    .Result;
                approveResponse.EnsureSuccessStatusCode();
            }

            var scenario = Scenario.Create("list_advances", async context =>
            {
                var request = new HttpRequestMessage(HttpMethod.Get,
                    $"/api/v1/advance/creator/{creatorId}?pageNumber=1&pageSize=10");

                var response = await httpClient.SendAsync(request);
                return Response.Ok(statusCode: response.StatusCode.ToString());
            })
            .WithLoadSimulations(
                Simulation.Inject(rate: RatePerSecond,
                                interval: TimeSpan.FromSeconds(1),
                                during: TimeSpan.FromSeconds(DurationSeconds))
            );

            var stats = NBomberRunner
                .RegisterScenarios(scenario)
                .WithTestName("Advance Listing Load Test")
                .WithTestSuite("API Load Tests")
                .Run();

            // Validations
            Assert.True(stats.AllOkCount > 0);
            Assert.True(stats.AllFailCount == 0, $"Found {stats.AllFailCount} failed requests");
            Assert.True(stats.ScenarioStats[0].Ok.Request.RPS >= RatePerSecond * 0.9);
        }

        [Fact]
        public void LoadTest_MixedOperations()
        {
            const int SimulationRPS = 100;
            const int CreateRPS = 50;
            const int ListRPS = 200;
            const int DurationSeconds = 60;

            var httpClient = _factory.CreateClient();
            var creatorId = Guid.NewGuid();

            var simulateScenario = Scenario.Create("simulate", async context =>
            {
                var amount = Random.Shared.Next(101, 10000);
                var request = new HttpRequestMessage(HttpMethod.Get,
                    $"/api/v1/advance/simulate?amount={amount}");

                var response = await httpClient.SendAsync(request);
                return Response.Ok(statusCode: response.StatusCode.ToString());
            })
            .WithLoadSimulations(
                Simulation.Inject(rate: SimulationRPS,
                                interval: TimeSpan.FromSeconds(1),
                                during: TimeSpan.FromSeconds(DurationSeconds))
            );

            var createScenario = Scenario.Create("create", async context =>
            {
                var dto = new CreateAdvanceRequestDto
                {
                    CreatorId = Guid.NewGuid(),
                    RequestedAmount = Random.Shared.Next(101, 10000)
                };

                var response = await httpClient.PostAsJsonAsync("/api/v1/advance", dto);
                return Response.Ok(statusCode: response.StatusCode.ToString());
            })
            .WithLoadSimulations(
                Simulation.Inject(rate: CreateRPS,
                                interval: TimeSpan.FromSeconds(1),
                                during: TimeSpan.FromSeconds(DurationSeconds))
            );

            var listScenario = Scenario.Create("list", async context =>
            {
                var request = new HttpRequestMessage(HttpMethod.Get,
                    $"/api/v1/advance/creator/{creatorId}?pageNumber=1&pageSize=10");

                var response = await httpClient.SendAsync(request);
                return Response.Ok(statusCode: response.StatusCode.ToString());
            })
            .WithLoadSimulations(
                Simulation.Inject(rate: ListRPS,
                                interval: TimeSpan.FromSeconds(1),
                                during: TimeSpan.FromSeconds(DurationSeconds))
            );

            var stats = NBomberRunner
                .RegisterScenarios(simulateScenario, createScenario, listScenario)
                .WithTestName("Mixed Operations Load Test")
                .WithTestSuite("API Load Tests")
                .Run();

            foreach (var scenarioStats in stats.ScenarioStats)
            {
                Assert.True(scenarioStats.Ok.Request.Count > 0);
                Assert.True(scenarioStats.Fail.Request.Count == 0,
                    $"Found {scenarioStats.Fail.Request.Count} failed requests in {scenarioStats.ScenarioName}");
            }
        }
    }
}