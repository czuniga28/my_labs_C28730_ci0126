using ExamTwo.DTOs;
using ExamTwo.Models;
using ExamTwo.Repositories;
using ExamTwo.Services;
using Moq;
using Xunit;

namespace ExamTwo.Tests.Services
{
    public class CoffeeMachineServiceTests
    {
        private readonly Mock<ICoffeeRepository> _mockCoffeeRepository;
        private readonly Mock<IChangeRepository> _mockChangeRepository;
        private readonly CoffeeMachineService _service;

        public CoffeeMachineServiceTests()
        {
            _mockCoffeeRepository = new Mock<ICoffeeRepository>();
            _mockChangeRepository = new Mock<IChangeRepository>();
            _service = new CoffeeMachineService(_mockCoffeeRepository.Object, _mockChangeRepository.Object);
        }

        [Fact]
        public void GetAvailableCoffees_ShouldReturnAllCoffees()
        {
            // Arrange
            var coffees = new List<CoffeeType>
            {
                new CoffeeType { Name = "Americano", Price = 950, Quantity = 10 },
                new CoffeeType { Name = "Capuchino", Price = 1200, Quantity = 8 }
            };

            _mockCoffeeRepository.Setup(r => r.GetAll()).Returns(coffees);

            // Act
            var result = _service.GetAvailableCoffees();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("Americano", result[0].Name);
            Assert.Equal(950, result[0].Price);
            Assert.Equal(10, result[0].Quantity);
            Assert.True(result[0].IsAvailable);
        }

        [Fact]
        public void CalculateTotalCost_WithValidOrder_ShouldReturnCorrectTotal()
        {
            // Arrange
            var order = new Dictionary<string, int>
            {
                { "Americano", 2 },
                { "Capuchino", 1 }
            };

            _mockCoffeeRepository.Setup(r => r.GetByName("Americano"))
                .Returns(new CoffeeType { Name = "Americano", Price = 950, Quantity = 10 });
            _mockCoffeeRepository.Setup(r => r.GetByName("Capuchino"))
                .Returns(new CoffeeType { Name = "Capuchino", Price = 1200, Quantity = 8 });

            // Act
            var result = _service.CalculateTotalCost(order);

            // Assert
            Assert.Equal(3100, result); // (950 * 2) + (1200 * 1)
        }

        [Fact]
        public void CalculateTotalCost_WithEmptyOrder_ShouldReturnZero()
        {
            // Arrange
            var order = new Dictionary<string, int>();

            // Act
            var result = _service.CalculateTotalCost(order);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void ProcessPurchase_WithValidOrderAndPayment_ShouldReturnSuccess()
        {
            // Arrange
            var order = new Dictionary<string, int>
            {
                { "Americano", 1 }
            };

            var payment = new Payment
            {
                TotalAmount = 1000,
                Coins = new List<int> { 500, 500 },
                Bills = new List<int>()
            };

            var coffee = new CoffeeType { Name = "Americano", Price = 950, Quantity = 10 };

            _mockCoffeeRepository.Setup(r => r.GetByName("Americano")).Returns(coffee);
            _mockCoffeeRepository.Setup(r => r.HasEnoughQuantity("Americano", 1)).Returns(true);
            _mockChangeRepository.Setup(r => r.CanMakeChange(50)).Returns(true);
            _mockChangeRepository.Setup(r => r.CalculateChangeBreakdown(50))
                .Returns(new Dictionary<int, int> { { 50, 1 } });
            _mockChangeRepository.Setup(r => r.DeductCoins(It.IsAny<Dictionary<int, int>>())).Returns(true);

            // Act
            var result = _service.ProcessPurchase(order, payment);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Change);
            Assert.Equal(50, result.Change.TotalAmount);
            _mockCoffeeRepository.Verify(r => r.UpdateQuantity("Americano", 9), Times.Once);
        }

        [Fact]
        public void ProcessPurchase_WithEmptyOrder_ShouldReturnFailure()
        {
            // Arrange
            var order = new Dictionary<string, int>();
            var payment = new Payment { TotalAmount = 1000, Coins = new List<int>(), Bills = new List<int>() };

            // Act
            var result = _service.ProcessPurchase(order, payment);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("vacía", result.Message);
        }

        [Fact]
        public void ProcessPurchase_WithInsufficientPayment_ShouldReturnFailure()
        {
            // Arrange
            var order = new Dictionary<string, int>
            {
                { "Americano", 1 }
            };

            var payment = new Payment
            {
                TotalAmount = 500,
                Coins = new List<int> { 500 },
                Bills = new List<int>()
            };

            var coffee = new CoffeeType { Name = "Americano", Price = 950, Quantity = 10 };

            _mockCoffeeRepository.Setup(r => r.GetByName("Americano")).Returns(coffee);
            _mockCoffeeRepository.Setup(r => r.HasEnoughQuantity("Americano", 1)).Returns(true);

            // Act
            var result = _service.ProcessPurchase(order, payment);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("insuficiente", result.Message);
        }

        [Fact]
        public void ProcessPurchase_WithInsufficientQuantity_ShouldReturnFailure()
        {
            // Arrange
            var order = new Dictionary<string, int>
            {
                { "Americano", 15 }
            };

            var payment = new Payment
            {
                TotalAmount = 15000,
                Coins = new List<int>(),
                Bills = new List<int> { 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000 }
            };

            var coffee = new CoffeeType { Name = "Americano", Price = 950, Quantity = 10 };

            _mockCoffeeRepository.Setup(r => r.GetByName("Americano")).Returns(coffee);
            _mockCoffeeRepository.Setup(r => r.HasEnoughQuantity("Americano", 15)).Returns(false);

            // Act
            var result = _service.ProcessPurchase(order, payment);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("suficientes", result.Message);
        }

        [Fact]
        public void ProcessPurchase_WithInsufficientChange_ShouldReturnFailure()
        {
            // Arrange
            var order = new Dictionary<string, int>
            {
                { "Americano", 1 }
            };

            var payment = new Payment
            {
                TotalAmount = 1000,
                Coins = new List<int> { 500, 500 },
                Bills = new List<int>()
            };

            var coffee = new CoffeeType { Name = "Americano", Price = 950, Quantity = 10 };

            _mockCoffeeRepository.Setup(r => r.GetByName("Americano")).Returns(coffee);
            _mockCoffeeRepository.Setup(r => r.HasEnoughQuantity("Americano", 1)).Returns(true);
            _mockChangeRepository.Setup(r => r.CanMakeChange(50)).Returns(false);

            // Act
            var result = _service.ProcessPurchase(order, payment);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("cambio", result.Message);
        }

        [Fact]
        public void ProcessPurchase_WithExactPayment_ShouldReturnSuccessWithNoChange()
        {
            // Arrange
            var order = new Dictionary<string, int>
            {
                { "Americano", 1 }
            };

            var payment = new Payment
            {
                TotalAmount = 950,
                Coins = new List<int> { 500, 100, 100, 100, 100, 50 },
                Bills = new List<int>()
            };

            var coffee = new CoffeeType { Name = "Americano", Price = 950, Quantity = 10 };

            _mockCoffeeRepository.Setup(r => r.GetByName("Americano")).Returns(coffee);
            _mockCoffeeRepository.Setup(r => r.HasEnoughQuantity("Americano", 1)).Returns(true);

            // Act
            var result = _service.ProcessPurchase(order, payment);

            // Assert
            Assert.True(result.Success);
            Assert.Null(result.Change);
        }
    }
}

