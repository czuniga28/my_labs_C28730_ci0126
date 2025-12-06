namespace ExamTwo.DTOs
{
    public class OrderRequestDto
    {
        public Dictionary<string, int> Order { get; set; } = new Dictionary<string, int>();
        public PaymentDto Payment { get; set; } = new PaymentDto();
    }

    public class PaymentDto
    {
        public int TotalAmount { get; set; }
        public List<int> Coins { get; set; } = new List<int>();
        public List<int> Bills { get; set; } = new List<int>();
    }
}

