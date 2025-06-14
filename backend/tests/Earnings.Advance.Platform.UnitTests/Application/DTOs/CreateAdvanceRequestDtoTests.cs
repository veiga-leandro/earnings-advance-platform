using Earnings.Advance.Platform.Application.DTOs.Advance;
using System.ComponentModel.DataAnnotations;

namespace Earnings.Advance.Platform.UnitTests.Application.DTOs
{
    public class CreateAdvanceRequestDtoTests
    {
        [Fact]
        public void Validate_WhenValid_ShouldPass()
        {
            // Arrange
            var dto = new CreateAdvanceRequestDto
            {
                CreatorId = Guid.NewGuid(),
                RequestedAmount = 1000m
            };

            // Act
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(dto,
                new ValidationContext(dto), validationResults, true);

            // Assert
            Assert.True(isValid);
            Assert.Empty(validationResults);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(100)] // Exatamente o mínimo também é inválido
        public void Validate_WhenAmountBelowMinimum_ShouldFail(decimal amount)
        {
            // Arrange
            var dto = new CreateAdvanceRequestDto
            {
                CreatorId = Guid.NewGuid(),
                RequestedAmount = amount
            };

            // Act
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(dto,
                new ValidationContext(dto), validationResults, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults,
                v => v.ErrorMessage.Contains("Amount must be bigger than"));
        }

        [Fact]
        public void Validate_WhenCreatorIdEmpty_ShouldFail()
        {
            // Arrange
            var dto = new CreateAdvanceRequestDto
            {
                CreatorId = Guid.Empty,
                RequestedAmount = 1000m
            };

            // Act
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(dto,
                new ValidationContext(dto), validationResults, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults,
                v => v.ErrorMessage.Contains("Creator ID is required"));
        }
    }
}
