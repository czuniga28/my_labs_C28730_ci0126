namespace ExamTwo.DTOs
{
    public class CoffeeTypeDto
    {
        public string Name { get; set; } = string.Empty;
        public int Price { get; set; }
        public int Quantity { get; set; }
        public bool IsAvailable { get; set; }
    }
}

