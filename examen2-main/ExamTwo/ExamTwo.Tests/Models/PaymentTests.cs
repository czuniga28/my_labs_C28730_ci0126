using ExamTwo.Models;
using ExamTwo.Services;
using Xunit;

namespace ExamTwo.Tests.Models
{
    public class PaymentValidatorTests
    {
        [Fact]
        public void IsValid_WithValidCoinsAndBills_ShouldReturnTrue()
        {
            // Arrange
            var payment = new Payment
            {
                TotalAmount = 1000,
                Coins = new List<int> { 500, 500 },
                Bills = new List<int> { 1000 }
            };

            // Act
            var result = PaymentValidator.IsValid(payment);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsValid_WithInvalidCoin_ShouldReturnFalse()
        {
            // Arrange
            var payment = new Payment
            {
                TotalAmount = 1000,
                Coins = new List<int> { 999 }, // Invalid denomination
                Bills = new List<int>()
            };

            // Act
            var result = PaymentValidator.IsValid(payment);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsValid_WithInvalidBill_ShouldReturnFalse()
        {
            // Arrange
            var payment = new Payment
            {
                TotalAmount = 1000,
                Coins = new List<int>(),
                Bills = new List<int> { 5000 } // Invalid denomination
            };

            // Act
            var result = PaymentValidator.IsValid(payment);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsValid_WithZeroTotalAmount_ShouldReturnFalse()
        {
            // Arrange
            var payment = new Payment
            {
                TotalAmount = 0,
                Coins = new List<int> { 500 },
                Bills = new List<int>()
            };

            // Act
            var result = PaymentValidator.IsValid(payment);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void CalculateTotal_ShouldReturnSumOfCoinsAndBills()
        {
            // Arrange
            var payment = new Payment
            {
                TotalAmount = 2000,
                Coins = new List<int> { 500, 100, 50, 25 },
                Bills = new List<int> { 1000 }
            };

            // Act
            var result = PaymentValidator.CalculateTotal(payment);

            // Assert
            Assert.Equal(1675, result); // 500 + 100 + 50 + 25 + 1000
        }

        [Fact]
        public void IsValidCoinDenomination_WithValidDenomination_ShouldReturnTrue()
        {
            // Act & Assert
            Assert.True(PaymentValidator.IsValidCoinDenomination(500));
            Assert.True(PaymentValidator.IsValidCoinDenomination(100));
            Assert.True(PaymentValidator.IsValidCoinDenomination(50));
            Assert.True(PaymentValidator.IsValidCoinDenomination(25));
        }

        [Fact]
        public void IsValidCoinDenomination_WithInvalidDenomination_ShouldReturnFalse()
        {
            // Act & Assert
            Assert.False(PaymentValidator.IsValidCoinDenomination(999));
            Assert.False(PaymentValidator.IsValidCoinDenomination(1000));
        }

        [Fact]
        public void IsValidBillDenomination_WithValidDenomination_ShouldReturnTrue()
        {
            // Act & Assert
            Assert.True(PaymentValidator.IsValidBillDenomination(1000));
        }

        [Fact]
        public void IsValidBillDenomination_WithInvalidDenomination_ShouldReturnFalse()
        {
            // Act & Assert
            Assert.False(PaymentValidator.IsValidBillDenomination(500));
            Assert.False(PaymentValidator.IsValidBillDenomination(5000));
        }
    }
}

