using Earnings.Advance.Platform.Domain.Constants;
using Earnings.Advance.Platform.Domain.Entities;
using Earnings.Advance.Platform.Domain.Enums;

namespace Earnings.Advance.Platform.UnitTests.Domain.Entities
{
    public class AdvanceRequestTests
    {
        [Fact]
        public void Constructor_WhenValidParameters_ShouldCreateSuccessfully()
        {
            // Arrange
            var creatorId = Guid.NewGuid();
            var amount = 1000m;
            var requestDate = DateTime.UtcNow;

            // Act
            var request = new AdvanceRequest(creatorId, amount, requestDate);

            // Assert
            Assert.NotEqual(Guid.Empty, request.Id);
            Assert.Equal(creatorId, request.CreatorId);
            Assert.Equal(amount, request.RequestedAmount);
            Assert.Equal(requestDate, request.RequestDate);
            Assert.Equal(AdvanceStatus.Pending, request.Status);
            Assert.Equal(amount * AdvanceConstants.FEE_RATE, request.Fee);
            Assert.Equal(amount - (amount * AdvanceConstants.FEE_RATE), request.NetAmount);
            Assert.Null(request.ProcessedDate);
        }

        [Fact]
        public void Approve_WhenPending_ShouldUpdateStatusAndDate()
        {
            // Arrange
            var request = new AdvanceRequest(Guid.NewGuid(), 1000m, DateTime.UtcNow);
            var beforeApprove = DateTime.UtcNow;

            // Act
            request.Approve();

            // Assert
            Assert.Equal(AdvanceStatus.Approved, request.Status);
            Assert.NotNull(request.ProcessedDate);
            Assert.True(request.ProcessedDate >= beforeApprove);
        }

        [Fact]
        public void Reject_WhenPending_ShouldUpdateStatusAndDate()
        {
            // Arrange
            var request = new AdvanceRequest(Guid.NewGuid(), 1000m, DateTime.UtcNow);
            var beforeReject = DateTime.UtcNow;

            // Act
            request.Reject();

            // Assert
            Assert.Equal(AdvanceStatus.Rejected, request.Status);
            Assert.NotNull(request.ProcessedDate);
            Assert.True(request.ProcessedDate >= beforeReject);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-1000)]
        public void Constructor_WhenAmountIsZeroOrNegative_ShouldThrowException(decimal amount)
        {
            // Arrange
            var creatorId = Guid.NewGuid();

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(
                () => new AdvanceRequest(creatorId, amount, DateTime.UtcNow));

            Assert.Contains("amount", exception.Message.ToLower());
        }

        [Fact]
        public void Approve_WhenAlreadyApproved_ShouldThrowException()
        {
            // Arrange
            var request = new AdvanceRequest(Guid.NewGuid(), 1000m, DateTime.UtcNow);
            request.Approve();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(
                () => request.Approve());

            Assert.Equal("Only pending requests should be approved", exception.Message);
        }

        [Fact]
        public void Reject_WhenAlreadyRejected_ShouldThrowException()
        {
            // Arrange
            var request = new AdvanceRequest(Guid.NewGuid(), 1000m, DateTime.UtcNow);
            request.Reject();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(
                () => request.Reject());

            Assert.Equal("Only pending requests should be rejected", exception.Message);
        }
    }
}
