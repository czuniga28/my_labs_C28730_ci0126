using ExamTwo.Models;

namespace ExamTwo.Repositories
{
    public class CoffeeRepository : ICoffeeRepository
    {
        private readonly Dictionary<string, CoffeeType> _coffees;

        public CoffeeRepository()
        {
            _coffees = new Dictionary<string, CoffeeType>
            {
                { "Americano", new CoffeeType { Name = "Americano", Price = 950, Quantity = 10 } },
                { "Capuchino", new CoffeeType { Name = "Capuchino", Price = 1200, Quantity = 8 } },
                { "Late", new CoffeeType { Name = "Late", Price = 1350, Quantity = 10 } },
                { "Mocachino", new CoffeeType { Name = "Mocachino", Price = 1500, Quantity = 15 } }
            };
        }

        public List<CoffeeType> GetAll()
        {
            return _coffees.Values.ToList();
        }

        public CoffeeType? GetByName(string name)
        {
            return _coffees.TryGetValue(name, out var coffee) ? coffee : null;
        }

        public bool UpdateQuantity(string name, int quantity)
        {
            if (!_coffees.ContainsKey(name))
                return false;

            _coffees[name].Quantity = quantity;
            return true;
        }

        public bool HasEnoughQuantity(string name, int requestedQuantity)
        {
            var coffee = GetByName(name);
            return coffee != null && coffee.Quantity >= requestedQuantity;
        }
    }
}

