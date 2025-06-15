using Earnings.Advance.Platform.Application.DTOs.Advance;
using Earnings.Advance.Platform.Application.DTOs.Common;
using Earnings.Advance.Platform.Application.DTOs.Simulation;
using Earnings.Advance.Platform.Domain.Constants;
using Earnings.Advance.Platform.IntegrationTests.TestBase;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net;
using System.Net.Http.Json;

namespace Earnings.Advance.Platform.IntegrationTests.Controllers.V1
{
    public class AdvanceControllerTests : IntegrationTestBase
    {
        public AdvanceControllerTests(WebApplicationFactory<Program> factory)
            : base(factory)
        {
        }

        [Fact]
        public async Task CreateAdvance_WhenValidRequest_ShouldReturnCreated()
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
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            result.Should().NotBeNull();
            result!.CreatorId.Should().Be(request.CreatorId);
            result.RequestedAmount.Should().Be(request.RequestedAmount);
            result.Status.Should().Be("pending");
            result.Fee.Should().Be(request.RequestedAmount * AdvanceConstants.FEE_RATE);
        }

        [Fact]
        public async Task CreateAdvance_WhenAmountBelowMinimum_ShouldReturnBadRequest()
        {
            // Arrange
            var request = new CreateAdvanceRequestDto
            {
                CreatorId = Guid.NewGuid(),
                RequestedAmount = 50m
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/v1/advance", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetByCreatorId_WhenValidRequest_ShouldReturnPagedResult()
        {
            // Arrange
            var creatorId = Guid.NewGuid();

            // Create some advances first
            var createRequest = new CreateAdvanceRequestDto
            {
                CreatorId = creatorId,
                RequestedAmount = 1000m
            };
            await _client.PostAsJsonAsync("/api/v1/advance", createRequest);

            // Act
            var response = await _client.GetAsync($"/api/v1/advance/creator/{creatorId}?pageNumber=1&pageSize=10");
            var result = await DeserializeResponse<PagedResultDto<AdvanceRequestResponseDto>>(response);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should().NotBeNull();
            result!.Items.Should().NotBeEmpty();
            result.CurrentPage.Should().Be(1);
            result.PageSize.Should().Be(10);
        }

        [Fact]
        public async Task Simulate_WhenValidAmount_ShouldReturnCalculation()
        {
            // Arrange
            var amount = 1000m;

            // Act
            var response = await _client.GetAsync($"/api/v1/advance/simulate?amount={amount}");
            var result = await DeserializeResponse<SimulationResponseDto>(response);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should().NotBeNull();
            result!.RequestedAmount.Should().Be(amount);
            result.Fee.Should().Be(amount * AdvanceConstants.FEE_RATE);
            result.NetAmount.Should().Be(amount - amount * AdvanceConstants.FEE_RATE);
        }

        [Fact]
        public async Task ApproveRequest_WhenValidRequest_ShouldReturnSuccess()
        {
            // Arrange
            var createRequest = new CreateAdvanceRequestDto
            {
                CreatorId = Guid.NewGuid(),
                RequestedAmount = 1000m
            };
            var createResponse = await _client.PostAsJsonAsync("/api/v1/advance", createRequest);
            var createdAdvance = await DeserializeResponse<AdvanceRequestResponseDto>(createResponse);

            // Act
            var response = await _client.PatchAsync($"/api/v1/advance/{createdAdvance!.Id}/approve", null);
            var result = await DeserializeResponse<AdvanceRequestResponseDto>(response);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should().NotBeNull();
            result!.Status.Should().Be("approved");
            result.ProcessedDate.Should().NotBeNull();
        }

        [Fact]
        public async Task RejectRequest_WhenValidRequest_ShouldReturnSuccess()
        {
            // Arrange
            var createRequest = new CreateAdvanceRequestDto
            {
                CreatorId = Guid.NewGuid(),
                RequestedAmount = 1000m
            };
            var createResponse = await _client.PostAsJsonAsync("/api/v1/advance", createRequest);
            var createdAdvance = await DeserializeResponse<AdvanceRequestResponseDto>(createResponse);

            // Act
            var response = await _client.PatchAsync($"/api/v1/advance/{createdAdvance!.Id}/reject", null);
            var result = await DeserializeResponse<AdvanceRequestResponseDto>(response);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should().NotBeNull();
            result!.Status.Should().Be("rejected");
            result.ProcessedDate.Should().NotBeNull();
        }
    }
}