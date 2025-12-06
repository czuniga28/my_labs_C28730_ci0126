using ExamTwo.Const;
using ExamTwo.DTOs;
using ExamTwo.Models;
using ExamTwo.Repositories;

namespace ExamTwo.Services
{
    public class CoffeeMachineService : ICoffeeMachineService
    {
        private readonly ICoffeeRepository _coffeeRepository;
        private readonly IChangeRepository _changeRepository;

        public CoffeeMachineService(ICoffeeRepository coffeeRepository, IChangeRepository changeRepository)
        {
            _coffeeRepository = coffeeRepository;
            _changeRepository = changeRepository;
        }

        public List<CoffeeTypeDto> GetAvailableCoffees()
        {
            return _coffeeRepository.GetAll()
                .Select(c => new CoffeeTypeDto
                {
                    Name = c.Name,
                    Price = c.Price,
                    Quantity = c.Quantity,
                    IsAvailable = c.IsAvailable
                })
                .ToList();
        }

        public int CalculateTotalCost(Dictionary<string, int> order)
        {
            if (order == null || order.Count == Constants.ZERO)
                return Constants.ZERO;

            int total = Constants.ZERO;
            foreach (var item in order)
            {
                var coffee = _coffeeRepository.GetByName(item.Key);
                if (coffee != null)
                {
                    total += coffee.Price * item.Value;
                }
            }

            return total;
        }

        public PurchaseResultDto ProcessPurchase(Dictionary<string, int> order, Payment payment)
        {
            var validationResult = ValidatePurchaseRequest(order, payment);
            if (validationResult != null)
                return validationResult;

            var totalCost = CalculateTotalCost(order);
            var paymentAmount = PaymentValidator.CalculateTotal(payment);
            var changeAmount = paymentAmount - totalCost;

            var paymentValidationResult = ValidatePaymentAmount(paymentAmount, totalCost, changeAmount);
            if (paymentValidationResult != null)
                return paymentValidationResult;

            UpdateCoffeeStock(order);

            var changeDto = ProcessChange(changeAmount, order);
            if (changeAmount > Constants.ZERO && changeDto == null)
                return CreateFailureResult(Constants.ServiceErrorMessages.INSUFFICIENT_CHANGE);

            return CreateSuccessResult(changeDto);
        }

        private PurchaseResultDto? ValidatePurchaseRequest(Dictionary<string, int> order, Payment payment)
        {
            var orderValidation = ValidateOrder(order);
            if (!orderValidation.IsValid)
            {
                return CreateFailureResult(orderValidation.ErrorMessage);
            }

            if (!PaymentValidator.IsValid(payment))
            {
                return CreateFailureResult(Constants.ServiceErrorMessages.INVALID_PAYMENT_DENOMINATIONS);
            }

            return null;
        }

        private PurchaseResultDto? ValidatePaymentAmount(int paymentAmount, int totalCost, int changeAmount)
        {
            if (paymentAmount < totalCost)
            {
                return CreateFailureResult(Constants.ServiceErrorMessages.INSUFFICIENT_MONEY);
            }

            if (changeAmount > Constants.ZERO && !_changeRepository.CanMakeChange(changeAmount))
            {
                return CreateFailureResult(Constants.ServiceErrorMessages.INSUFFICIENT_CHANGE);
            }

            return null;
        }

        private void UpdateCoffeeStock(Dictionary<string, int> order)
        {
            foreach (var (coffeeName, quantity) in order)
            {
                var coffee = _coffeeRepository.GetByName(coffeeName);
                if (coffee != null)
                {
                    var newQuantity = coffee.Quantity - quantity;
                    _coffeeRepository.UpdateQuantity(coffeeName, newQuantity);
                }
            }
        }

        private ChangeBreakdownDto? ProcessChange(int changeAmount, Dictionary<string, int> order)
        {
            if (changeAmount <= Constants.ZERO)
                return null;

            var changeBreakdown = _changeRepository.CalculateChangeBreakdown(changeAmount);

            if (!_changeRepository.DeductCoins(changeBreakdown))
            {
                RollbackCoffeeStock(order);
                return null;
            }

            return new ChangeBreakdownDto
            {
                TotalAmount = changeAmount,
                Breakdown = changeBreakdown,
                FormattedBreakdown = FormatChangeBreakdown(changeAmount, changeBreakdown)
            };
        }

        private void RollbackCoffeeStock(Dictionary<string, int> order)
        {
            foreach (var (coffeeName, quantity) in order)
            {
                var coffee = _coffeeRepository.GetByName(coffeeName);
                if (coffee != null)
                {
                    _coffeeRepository.UpdateQuantity(coffeeName, coffee.Quantity + quantity);
                }
            }
        }

        private PurchaseResultDto CreateFailureResult(string message)
        {
            return new PurchaseResultDto
            {
                Success = false,
                Message = message
            };
        }

        private PurchaseResultDto CreateSuccessResult(ChangeBreakdownDto? changeDto)
        {
            return new PurchaseResultDto
            {
                Success = true,
                Message = Constants.SuccessMessages.PURCHASE_SUCCESS,
                Change = changeDto
            };
        }

        private (bool IsValid, string ErrorMessage) ValidateOrder(Dictionary<string, int> order)
        {
            if (order == null || order.Count == Constants.ZERO)
            {
                return (false, Constants.ServiceErrorMessages.ORDER_EMPTY);
            }

            foreach (var item in order)
            {
                if (item.Value <= Constants.MIN_QUANTITY)
                {
                    return (false, string.Format(Constants.ServiceErrorMessages.QUANTITY_MUST_BE_GREATER_THAN_ZERO, item.Key));
                }

                var coffee = _coffeeRepository.GetByName(item.Key);
                if (coffee == null)
                {
                    return (false, string.Format(Constants.ServiceErrorMessages.COFFEE_TYPE_NOT_EXISTS, item.Key));
                }

                if (!_coffeeRepository.HasEnoughQuantity(item.Key, item.Value))
                {
                    return (false, string.Format(Constants.ServiceErrorMessages.INSUFFICIENT_COFFEE_STOCK, item.Key));
                }
            }

            return (true, string.Empty);
        }

        private string FormatChangeBreakdown(int totalAmount, Dictionary<int, int> breakdown)
        {
            var lines = new List<string>
            {
                string.Format(Constants.ChangeMessages.CHANGE_AMOUNT, totalAmount),
                Constants.ChangeMessages.BREAKDOWN_HEADER
            };

            foreach (var (denomination, quantity) in breakdown.OrderByDescending(k => k.Key))
            {
                if (quantity > Constants.ZERO)
                {
                    var coinWord = quantity > 1 ? Constants.ChangeMessages.COIN_PLURAL : Constants.ChangeMessages.COIN_SINGULAR;
                    lines.Add(string.Format(Constants.ChangeMessages.COIN_FORMAT, quantity, coinWord, denomination));
                }
            }

            return string.Join("\n", lines);
        }
    }
}

