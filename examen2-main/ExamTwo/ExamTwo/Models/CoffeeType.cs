namespace ExamTwo.Models
{
    public class CoffeeType
    {
        public string Name { get; set; } = string.Empty;
        public int Price { get; set; }
        public int Quantity { get; set; }

        public bool IsAvailable => Quantity > 0;
    }
}

