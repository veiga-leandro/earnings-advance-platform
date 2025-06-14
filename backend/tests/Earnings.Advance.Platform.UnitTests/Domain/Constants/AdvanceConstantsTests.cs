using Earnings.Advance.Platform.Domain.Constants;

namespace Earnings.Advance.Platform.UnitTests.Domain.Constants
{
    public class AdvanceConstantsTests
    {
        [Theory]
        [InlineData(0, "R$ 0,00")]
        [InlineData(1, "R$ 1,00")]
        [InlineData(1.5, "R$ 1,50")]
        [InlineData(1000, "R$ 1000,00")]
        [InlineData(1000.99, "R$ 1000,99")]
        public void FormatAmount_WithVariousValues_ShouldFormatCorrectly(decimal amount, string expected)
        {
            // Act
            var result = AdvanceConstants.FormatAmount(amount);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Constants_ShouldHaveCorrectValues()
        {
            Assert.Equal(0.05m, AdvanceConstants.FEE_RATE);
            Assert.Equal(100.00m, AdvanceConstants.MINIMUM_AMOUNT);
            Assert.Equal("R$", AdvanceConstants.CURRENCY_SYMBOL);
        }
    }
}
