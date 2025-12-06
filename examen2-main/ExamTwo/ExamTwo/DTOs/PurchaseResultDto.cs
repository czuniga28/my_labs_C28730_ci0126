namespace ExamTwo.DTOs
{
    public class PurchaseResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public ChangeBreakdownDto? Change { get; set; }
    }

    public class ChangeBreakdownDto
    {
        public int TotalAmount { get; set; }
        public Dictionary<int, int> Breakdown { get; set; } = new Dictionary<int, int>();
        public string FormattedBreakdown { get; set; } = string.Empty;
    }
}

