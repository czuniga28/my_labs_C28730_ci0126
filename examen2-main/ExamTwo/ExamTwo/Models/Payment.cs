namespace ExamTwo.Models
{
    public class Payment
    {
        public int TotalAmount { get; set; }
        public List<int> Coins { get; set; } = new List<int>();
        public List<int> Bills { get; set; } = new List<int>();
    }
}

