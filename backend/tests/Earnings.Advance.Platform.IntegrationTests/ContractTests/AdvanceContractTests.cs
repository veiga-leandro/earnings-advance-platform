using Earnings.Advance.Platform.Application.DTOs.Advance;
using Earnings.Advance.Platform.Application.DTOs.Common;
using Earnings.Advance.Platform.Application.DTOs.Simulation;
using Earnings.Advance.Platform.IntegrationTests.TestBase;
using System.Net;
using System.Net.Http.Json;

namespace Earnings.Advance.Platform.IntegrationTests.ContractTests
{
    public class AdvanceContractTests : IntegrationTestBase, IClassFixture<CustomWebApplicationFactory>
    {
        public AdvanceContractTests(CustomWebApplicationFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task CreateAdvance_Contract_ShouldMatchSpecification()
        {
            // Arrange
            var request = new CreateAdvanceRequestDto
            {
                CreatorId = Guid.NewGuid(),
                RequestedAmount = 1000m
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/v1/advance", request);
            var result = await DeserializeResponse<AdvanceRequestResponseDto>(response);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.NotNull(result);

            // Contract 
            Assert.NotEqual(Guid.Empty, result!.Id);
            Assert.Equal(request.CreatorId, result.CreatorId);
            Assert.Equal(request.RequestedAmount, result.RequestedAmount);
            Assert.True(result.Fee > 0);
            Assert.True(result.NetAmount > 0);
            Assert.Equal("pending", result.Status);
            Assert.NotEqual(default, result.RequestDate);
            Assert.Null(result.ProcessedDate);
        }

        [Fact]
        public async Task GetByCreatorId_Contract_ShouldMatchSpecification()
        {
            // Arrange
            var creatorId = Guid.NewGuid();
            await CreateSampleRequest(creatorId);

            // Act
            var response = await _client.GetAsync($"/api/v1/advance/creator/{creatorId}?pageNumber=1&pageSize=10");
            var result = await DeserializeResponse<PagedResultDto<AdvanceRequestResponseDto>>(response);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result);

            // Contract 
            Assert.NotNull(result!.Items);
            Assert.True(result.CurrentPage >= 1);
            Assert.True(result.PageSize > 0);
            Assert.True(result.TotalPages >= 0);
            Assert.True(result.TotalCount >= 0);

            // Validate Each Item in the Collection
            foreach (var item in result.Items)
            {
                Assert.NotEqual(Guid.Empty, item.Id);
                Assert.Equal(creatorId, item.CreatorId);
                Assert.True(item.RequestedAmount > 0);
                Assert.True(item.Fee > 0);
                Assert.True(item.NetAmount > 0);
                Assert.NotNull(item.Status);
                Assert.NotEqual(default, item.RequestDate);
            }
        }

        [Fact]
        public async Task Simulate_Contract_ShouldMatchSpecification()
        {
            // Arrange
            var amount = 1000m;

            // Act
            var response = await _client.GetAsync($"/api/v1/advance/simulate?amount={amount}");
            var result = await DeserializeResponse<SimulationResponseDto>(response);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result);

            // Contract 
            Assert.Equal(amount, result!.RequestedAmount);
            Assert.True(result.Fee > 0);
            Assert.True(result.NetAmount > 0);
            Assert.True(result.FeeRate > 0);
            Assert.True(result.NetAmount == amount - result.Fee);
        }

        [Fact]
        public async Task ApproveRequest_Contract_ShouldMatchSpecification()
        {
            // Arrange
            var request = await CreateSampleRequest(Guid.NewGuid());

            // Act
            var response = await _client.PatchAsync($"/api/v1/advance/{request.Id}/approve", null);
            var result = await DeserializeResponse<AdvanceRequestResponseDto>(response);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result);

            // Contract 
            Assert.Equal(request.Id, result!.Id);
            Assert.Equal(request.CreatorId, result.CreatorId);
            Assert.Equal(request.RequestedAmount, result.RequestedAmount);
            Assert.Equal("approved", result.Status);
            Assert.NotNull(result.ProcessedDate);
        }

        [Fact]
        public async Task RejectRequest_Contract_ShouldMatchSpecification()
        {
            // Arrange
            var request = await CreateSampleRequest(Guid.NewGuid());

            // Act
            var response = await _client.PatchAsync($"/api/v1/advance/{request.Id}/reject", null);
            var result = await DeserializeResponse<AdvanceRequestResponseDto>(response);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result);

            // Contract 
            Assert.Equal(request.Id, result!.Id);
            Assert.Equal(request.CreatorId, result.CreatorId);
            Assert.Equal(request.RequestedAmount, result.RequestedAmount);
            Assert.Equal("rejected", result.Status);
            Assert.NotNull(result.ProcessedDate);
        }

        private async Task<AdvanceRequestResponseDto> CreateSampleRequest(Guid creatorId)
        {
            var request = new CreateAdvanceRequestDto
            {
                CreatorId = creatorId,
                RequestedAmount = 1000m
            };

            var response = await _client.PostAsJsonAsync("/api/v1/advance", request);
            return await DeserializeResponse<AdvanceRequestResponseDto>(response)
                ?? throw new InvalidOperationException("Failed to create sample request");
        }
    }
}