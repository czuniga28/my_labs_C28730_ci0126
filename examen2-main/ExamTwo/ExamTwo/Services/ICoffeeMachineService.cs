using ExamTwo.DTOs;
using ExamTwo.Models;

namespace ExamTwo.Services
{
    public interface ICoffeeMachineService
    {
        List<CoffeeTypeDto> GetAvailableCoffees();
        int CalculateTotalCost(Dictionary<string, int> order);
        PurchaseResultDto ProcessPurchase(Dictionary<string, int> order, Payment payment);
    }
}

