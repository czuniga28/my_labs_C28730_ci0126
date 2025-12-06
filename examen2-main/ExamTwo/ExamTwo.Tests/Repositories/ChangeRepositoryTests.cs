using ExamTwo.Repositories;
using Xunit;

namespace ExamTwo.Tests.Repositories
{
    public class ChangeRepositoryTests
    {
        [Fact]
        public void GetAvailableCoins_ShouldReturnAllCoins()
        {
            // Arrange
            var repository = new ChangeRepository();

            // Act
            var result = repository.GetAvailableCoins();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(4, result.Count);
            Assert.Equal(20, result[500]);
            Assert.Equal(30, result[100]);
            Assert.Equal(50, result[50]);
            Assert.Equal(25, result[25]);
        }

        [Fact]
        public void CanMakeChange_WithExactChange_ShouldReturnTrue()
        {
            // Arrange
            var repository = new ChangeRepository();

            // Act
            var result = repository.CanMakeChange(650);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void CanMakeChange_WithZeroAmount_ShouldReturnTrue()
        {
            // Arrange
            var repository = new ChangeRepository();

            // Act
            var result = repository.CanMakeChange(0);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void CanMakeChange_WithInsufficientCoins_ShouldReturnFalse()
        {
            // Arrange
            var repository = new ChangeRepository();
            // Try to make change for a very large amount
            var largeAmount = 100000;

            // Act
            var result = repository.CanMakeChange(largeAmount);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void CalculateChangeBreakdown_WithValidAmount_ShouldReturnCorrectBreakdown()
        {
            // Arrange
            var repository = new ChangeRepository();
            var amount = 650;

            // Act
            var result = repository.CalculateChangeBreakdown(amount);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.ContainsKey(500));
            Assert.True(result.ContainsKey(100));
            Assert.True(result.ContainsKey(50));
            Assert.Equal(1, result[500]);
            Assert.Equal(1, result[100]);
            Assert.Equal(1, result[50]);
        }

        [Fact]
        public void DeductCoins_WithValidCoins_ShouldDeductAndReturnTrue()
        {
            // Arrange
            var repository = new ChangeRepository();
            var coinsToDeduct = new Dictionary<int, int>
            {
                { 500, 1 },
                { 100, 1 }
            };
            var initialCoins = repository.GetAvailableCoins();

            // Act
            var result = repository.DeductCoins(coinsToDeduct);
            var coinsAfter = repository.GetAvailableCoins();

            // Assert
            Assert.True(result);
            Assert.Equal(initialCoins[500] - 1, coinsAfter[500]);
            Assert.Equal(initialCoins[100] - 1, coinsAfter[100]);
        }

        [Fact]
        public void DeductCoins_WithInsufficientCoins_ShouldReturnFalse()
        {
            // Arrange
            var repository = new ChangeRepository();
            var coinsToDeduct = new Dictionary<int, int>
            {
                { 500, 1000 } // More than available
            };

            // Act
            var result = repository.DeductCoins(coinsToDeduct);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void DeductCoins_WithInvalidDenomination_ShouldReturnFalse()
        {
            // Arrange
            var repository = new ChangeRepository();
            var coinsToDeduct = new Dictionary<int, int>
            {
                { 999, 1 } // Invalid denomination
            };

            // Act
            var result = repository.DeductCoins(coinsToDeduct);

            // Assert
            Assert.False(result);
        }
    }
}

