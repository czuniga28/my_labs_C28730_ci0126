using ExamTwo.Models;
using ExamTwo.Repositories;
using Xunit;

namespace ExamTwo.Tests.Repositories
{
    public class CoffeeRepositoryTests
    {
        [Fact]
        public void GetAll_ShouldReturnAllCoffeeTypes()
        {
            // Arrange
            var repository = new CoffeeRepository();

            // Act
            var result = repository.GetAll();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(4, result.Count);
            Assert.Contains(result, c => c.Name == "Americano");
            Assert.Contains(result, c => c.Name == "Capuchino");
            Assert.Contains(result, c => c.Name == "Late");
            Assert.Contains(result, c => c.Name == "Mocachino");
        }

        [Fact]
        public void GetByName_WithValidName_ShouldReturnCoffeeType()
        {
            // Arrange
            var repository = new CoffeeRepository();

            // Act
            var result = repository.GetByName("Americano");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Americano", result.Name);
            Assert.Equal(950, result.Price);
            Assert.Equal(10, result.Quantity);
        }

        [Fact]
        public void GetByName_WithInvalidName_ShouldReturnNull()
        {
            // Arrange
            var repository = new CoffeeRepository();

            // Act
            var result = repository.GetByName("InvalidCoffee");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void UpdateQuantity_WithValidName_ShouldUpdateQuantity()
        {
            // Arrange
            var repository = new CoffeeRepository();
            var newQuantity = 5;

            // Act
            var result = repository.UpdateQuantity("Americano", newQuantity);
            var coffee = repository.GetByName("Americano");

            // Assert
            Assert.True(result);
            Assert.NotNull(coffee);
            Assert.Equal(newQuantity, coffee.Quantity);
        }

        [Fact]
        public void UpdateQuantity_WithInvalidName_ShouldReturnFalse()
        {
            // Arrange
            var repository = new CoffeeRepository();

            // Act
            var result = repository.UpdateQuantity("InvalidCoffee", 5);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void HasEnoughQuantity_WithSufficientQuantity_ShouldReturnTrue()
        {
            // Arrange
            var repository = new CoffeeRepository();

            // Act
            var result = repository.HasEnoughQuantity("Americano", 5);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void HasEnoughQuantity_WithInsufficientQuantity_ShouldReturnFalse()
        {
            // Arrange
            var repository = new CoffeeRepository();

            // Act
            var result = repository.HasEnoughQuantity("Americano", 15);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void HasEnoughQuantity_WithExactQuantity_ShouldReturnTrue()
        {
            // Arrange
            var repository = new CoffeeRepository();

            // Act
            var result = repository.HasEnoughQuantity("Americano", 10);

            // Assert
            Assert.True(result);
        }
    }
}

