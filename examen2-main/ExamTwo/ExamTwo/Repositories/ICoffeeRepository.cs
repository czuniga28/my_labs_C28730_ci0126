using ExamTwo.Models;

namespace ExamTwo.Repositories
{
    public interface ICoffeeRepository
    {
        List<CoffeeType> GetAll();
        CoffeeType? GetByName(string name);
        bool UpdateQuantity(string name, int quantity);
        bool HasEnoughQuantity(string name, int requestedQuantity);
    }
}

