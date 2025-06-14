using Earnings.Advance.Platform.Application.DTOs.Advance;
using Earnings.Advance.Platform.Application.Services;
using Earnings.Advance.Platform.Domain.Constants;
using Earnings.Advance.Platform.Domain.Entities;
using Earnings.Advance.Platform.Domain.Exceptions;
using Earnings.Advance.Platform.Domain.Interfaces;
using Moq;

namespace Earnings.Advance.Platform.UnitTests.Application.Services
{
    public class AdvanceServiceTests
    {
        private readonly Mock<IAdvanceRepository> _repositoryMock;
        private readonly AdvanceService _service;

        public AdvanceServiceTests()
        {
            _repositoryMock = new Mock<IAdvanceRepository>();
            _service = new AdvanceService(_repositoryMock.Object);
        }

        #region Create Advance Tests
        [Theory]
        [InlineData(100.01)]  // Limite mínimo + 0.01
        [InlineData(1000000)] // Valor alto
        [InlineData(555.55)]  // Valor com decimais
        public async Task CreateAdvanceAsync_WhenValidAmounts_ShouldCalculateCorrectly(decimal amount)
        {
            // Arrange
            var dto = new CreateAdvanceRequestDto
            {
                CreatorId = Guid.NewGuid(),
                RequestedAmount = amount
            };

            _repositoryMock.Setup(x => x.HasPendingRequestAsync(dto.CreatorId))
                           .ReturnsAsync(false);

            _repositoryMock.Setup(x => x.CreateAsync(It.IsAny<AdvanceRequest>()))
                           .ReturnsAsync((AdvanceRequest ar) => ar);

            // Act
            var result = await _service.CreateAdvanceAsync(dto);

            // Assert
            Assert.Equal(amount * AdvanceConstants.FEE_RATE, result.Fee);
            Assert.Equal(amount - amount * AdvanceConstants.FEE_RATE, result.NetAmount);
        }
        [Fact]
        public async Task CreateAdvanceAsync_WhenValidRequest_ShouldCreateSuccessfully()
        {
            // Arrange
            var dto = new CreateAdvanceRequestDto
            {
                CreatorId = Guid.NewGuid(),
                RequestedAmount = 1000m
            };

            _repositoryMock.Setup(x => x.HasPendingRequestAsync(dto.CreatorId))
                          .ReturnsAsync(false);

            _repositoryMock.Setup(x => x.CreateAsync(It.IsAny<AdvanceRequest>()))
                          .ReturnsAsync((AdvanceRequest ar) => ar);

            // Act
            var result = await _service.CreateAdvanceAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(dto.CreatorId, result.CreatorId);
            Assert.Equal(dto.RequestedAmount, result.RequestedAmount);
            Assert.Equal(50m, result.Fee); // 5% de 1000
            Assert.Equal(950m, result.NetAmount); // 1000 - 50
            Assert.Equal("pending", result.Status);
        }

        [Fact]
        public async Task CreateAdvanceAsync_WhenCreatorIdEmpty_ShouldThrowArgumentException()
        {
            // Arrange
            var dto = new CreateAdvanceRequestDto
            {
                CreatorId = Guid.Empty,
                RequestedAmount = 1000m
            };

            _repositoryMock.Setup(x => x.HasPendingRequestAsync(dto.CreatorId))
                          .ReturnsAsync(false);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _service.CreateAdvanceAsync(dto));

            Assert.Equal("Creator ID is required (Parameter 'creatorId')", exception.Message);
        }

        [Fact]
        public async Task CreateAdvanceAsync_WhenCreatorHasPendingRequest_ShouldThrowBusinessException()
        {
            // Arrange
            var dto = new CreateAdvanceRequestDto
            {
                CreatorId = Guid.NewGuid(),
                RequestedAmount = 1000m
            };

            _repositoryMock.Setup(x => x.HasPendingRequestAsync(dto.CreatorId))
                          .ReturnsAsync(true);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BusinessException>(
                () => _service.CreateAdvanceAsync(dto));

            Assert.Equal("Creator already has a pending request", exception.Message);
        }

        [Fact]
        public async Task CreateAdvanceAsync_WhenAmountBelowMinimum_ShouldThrowArgumentException()
        {
            // Arrange
            var dto = new CreateAdvanceRequestDto
            {
                CreatorId = Guid.NewGuid(),
                RequestedAmount = AdvanceConstants.MINIMUM_AMOUNT - 1 // Garante estar abaixo do mínimo
            };

            _repositoryMock.Setup(x => x.HasPendingRequestAsync(dto.CreatorId))
                          .ReturnsAsync(false);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _service.CreateAdvanceAsync(dto));

            Assert.Equal("Requested amount must be bigger than R$ 100,00 (Parameter 'requestedAmount')", exception.Message);
        }
        #endregion

        #region Simulate Tests
        [Theory]
        [InlineData(100.01)]  // Limite mínimo + 0.01
        [InlineData(1000000)] // Valor alto
        [InlineData(555.55)]  // Valor com decimais
        public async Task SimulateAsync_WithVariousAmounts_ShouldCalculateCorrectly(decimal amount)
        {
            // Act
            var result = await _service.SimulateAsync(amount);

            // Assert
            Assert.Equal(amount, result.RequestedAmount);
            Assert.Equal(amount * AdvanceConstants.FEE_RATE, result.Fee);
            Assert.Equal(amount - amount * AdvanceConstants.FEE_RATE, result.NetAmount);
            Assert.Equal(AdvanceConstants.FEE_RATE, result.FeeRate);
        }

        [Fact]
        public async Task SimulateAsync_WhenValidAmount_ShouldCalculateCorrectly()
        {
            // Arrange
            var amount = 1000m;

            // Act
            var result = await _service.SimulateAsync(amount);

            // Assert
            Assert.Equal(amount, result.RequestedAmount);
            Assert.Equal(amount * AdvanceConstants.FEE_RATE, result.Fee);
            Assert.Equal(amount - amount * AdvanceConstants.FEE_RATE, result.NetAmount);
            Assert.Equal(AdvanceConstants.FEE_RATE, result.FeeRate);
        }

        [Fact]
        public async Task SimulateAsync_WhenAmountBelowMinimum_ShouldThrowArgumentException()
        {
            // Arrange
            var amount = AdvanceConstants.MINIMUM_AMOUNT / 2; // Abaixo do mínimo de R$ 100,00

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _service.SimulateAsync(amount));

            Assert.Equal("Amount must be bigger than R$ 100,00", exception.Message);
        }
        #endregion

        #region Get By Creator Id Tests
        [Fact]
        public async Task GetByCreatorIdAsync_WhenValidRequest_ShouldReturnPagedResult()
        {
            // Arrange
            var creatorId = Guid.NewGuid();
            var pageNumber = 1;
            var pageSize = 10;
            var skip = (pageNumber - 1) * pageSize;

            var requests = new List<AdvanceRequest>
            {
                new AdvanceRequest(creatorId, 1000m, DateTime.UtcNow),
                new AdvanceRequest(creatorId, 2000m, DateTime.UtcNow)
            };

            _repositoryMock.Setup(x => x.GetByCreatorIdAsync(creatorId, skip, pageSize))
                           .ReturnsAsync((requests, requests.Count));

            // Act
            var result = await _service.GetByCreatorIdAsync(creatorId, pageNumber, pageSize);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(pageNumber, result.CurrentPage);
            Assert.Equal(pageSize, result.PageSize);
            Assert.Equal(2, result.TotalCount);
            Assert.Equal(2, result.Items.Count());
            Assert.False(result.HasNext);
            Assert.False(result.HasPrevious);
        }

        [Fact]
        public async Task GetByCreatorIdAsync_WhenHasNextPage_ShouldIndicateCorrectly()
        {
            // Arrange
            var creatorId = Guid.NewGuid();
            var pageNumber = 1;
            var pageSize = 2;
            var skip = (pageNumber - 1) * pageSize;

            var requests = Enumerable.Range(0, 5)
                .Select(_ => new AdvanceRequest(creatorId, 1000m, DateTime.UtcNow))
                .ToList();

            _repositoryMock.Setup(x => x.GetByCreatorIdAsync(creatorId, skip, pageSize))
                           .ReturnsAsync((requests.Take(pageSize).ToList(), requests.Count));

            // Act
            var result = await _service.GetByCreatorIdAsync(creatorId, pageNumber, pageSize);

            // Assert
            Assert.True(result.HasNext);
            Assert.False(result.HasPrevious);
            Assert.Equal(3, result.TotalPages);
            Assert.Equal(5, result.TotalCount);
            Assert.Equal(2, result.Items.Count());
        }

        [Fact]
        public async Task GetByCreatorIdAsync_WhenHasPreviousPage_ShouldIndicateCorrectly()
        {
            // Arrange
            var creatorId = Guid.NewGuid();
            var pageNumber = 2;
            var pageSize = 2;
            var skip = (pageNumber - 1) * pageSize;

            var requests = Enumerable.Range(0, 5)
                .Select(_ => new AdvanceRequest(creatorId, 1000m, DateTime.UtcNow))
                .ToList();

            _repositoryMock.Setup(x => x.GetByCreatorIdAsync(creatorId, skip, pageSize))
                           .ReturnsAsync((requests.Skip(skip).Take(pageSize).ToList(), requests.Count));

            // Act
            var result = await _service.GetByCreatorIdAsync(creatorId, pageNumber, pageSize);

            // Assert
            Assert.True(result.HasNext);
            Assert.True(result.HasPrevious);
            Assert.Equal(3, result.TotalPages);
        }

        [Fact]
        public async Task GetByCreatorIdAsync_WhenNoRequests_ShouldReturnEmptyPage()
        {
            // Arrange
            var creatorId = Guid.NewGuid();
            var pageNumber = 1;
            var pageSize = 10;
            var skip = (pageNumber - 1) * pageSize;

            _repositoryMock.Setup(x => x.GetByCreatorIdAsync(creatorId, skip, pageSize))
                           .ReturnsAsync((new List<AdvanceRequest>(), 0));

            // Act
            var result = await _service.GetByCreatorIdAsync(creatorId, pageNumber, pageSize);

            // Assert
            Assert.Empty(result.Items);
            Assert.Equal(0, result.TotalCount);
            Assert.Equal(0, result.TotalPages);
            Assert.False(result.HasNext);
            Assert.False(result.HasPrevious);
        }
        #endregion

        #region Approve Tests
        [Fact]
        public async Task ApproveAsync_WhenRequestExists_ShouldApproveSuccessfully()
        {
            // Arrange
            var requestId = Guid.NewGuid();
            var request = new AdvanceRequest(Guid.NewGuid(), 1000m, DateTime.UtcNow);

            _repositoryMock.Setup(x => x.GetByIdAsync(requestId))
                           .ReturnsAsync(request);
            _repositoryMock.Setup(x => x.UpdateAsync(It.IsAny<AdvanceRequest>()))
                           .ReturnsAsync((AdvanceRequest ar) => ar);

            // Act
            var result = await _service.ApproveAsync(requestId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("approved", result.Status);
            Assert.NotNull(result.ProcessedDate);
        }

        [Fact]
        public async Task ApproveAsync_WhenRequestNotFound_ShouldThrowBusinessException()
        {
            // Arrange
            var requestId = Guid.NewGuid();

            _repositoryMock.Setup(x => x.GetByIdAsync(requestId))
                           .ReturnsAsync((AdvanceRequest?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BusinessException>(
                () => _service.ApproveAsync(requestId));

            Assert.Equal("Request not found", exception.Message);
        }

        [Fact]
        public async Task ApproveAsync_WhenRequestAlreadyProcessed_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var requestId = Guid.NewGuid();
            var request = new AdvanceRequest(Guid.NewGuid(), 1000m, DateTime.UtcNow);
            request.Approve(); // Set status to Approved

            _repositoryMock.Setup(x => x.GetByIdAsync(requestId))
                           .ReturnsAsync(request);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.ApproveAsync(requestId));

            Assert.Equal("Only pending requests should be approved", exception.Message);
        }
        #endregion

        #region Reject Tests
        [Fact]
        public async Task RejectAsync_WhenRequestExists_ShouldRejectSuccessfully()
        {
            // Arrange
            var requestId = Guid.NewGuid();
            var request = new AdvanceRequest(Guid.NewGuid(), 1000m, DateTime.UtcNow);

            _repositoryMock.Setup(x => x.GetByIdAsync(requestId))
                           .ReturnsAsync(request);
            _repositoryMock.Setup(x => x.UpdateAsync(It.IsAny<AdvanceRequest>()))
                           .ReturnsAsync((AdvanceRequest ar) => ar);

            // Act
            var result = await _service.RejectAsync(requestId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("rejected", result.Status);
            Assert.NotNull(result.ProcessedDate);
        }

        [Fact]
        public async Task RejectAsync_WhenRequestNotFound_ShouldThrowBusinessException()
        {
            // Arrange
            var requestId = Guid.NewGuid();

            _repositoryMock.Setup(x => x.GetByIdAsync(requestId))
                           .ReturnsAsync((AdvanceRequest?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BusinessException>(
                () => _service.RejectAsync(requestId));

            Assert.Equal("Request not found", exception.Message);
        }

        [Fact]
        public async Task RejectAsync_WhenRequestAlreadyProcessed_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var requestId = Guid.NewGuid();
            var request = new AdvanceRequest(Guid.NewGuid(), 1000m, DateTime.UtcNow);
            request.Approve(); // Set status to Approved

            _repositoryMock.Setup(x => x.GetByIdAsync(requestId))
                           .ReturnsAsync(request);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.RejectAsync(requestId));

            Assert.Equal("Only pending requests should be rejected", exception.Message);
        }
        #endregion
    }
}
