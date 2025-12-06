using ExamTwo.Models;

namespace ExamTwo.Services
{
    public static class PaymentValidator
    {
        public static readonly HashSet<int> ValidCoinDenominations = new() { 25, 50, 100, 500 };
        public static readonly HashSet<int> ValidBillDenominations = new() { 1000 };

        public static bool IsValid(Payment payment)
        {
            if (payment == null || payment.TotalAmount <= 0)
                return false;

            var allCoinsValid = payment.Coins?.All(c => IsValidCoinDenomination(c)) ?? true;
            var allBillsValid = payment.Bills?.All(b => IsValidBillDenomination(b)) ?? true;

            return allCoinsValid && allBillsValid;
        }

        public static int CalculateTotal(Payment payment)
        {
            if (payment == null)
                return 0;

            var coinsTotal = payment.Coins?.Sum() ?? 0;
            var billsTotal = payment.Bills?.Sum() ?? 0;

            return coinsTotal + billsTotal;
        }

        public static bool IsValidCoinDenomination(int denomination)
        {
            return ValidCoinDenominations.Contains(denomination);
        }

        public static bool IsValidBillDenomination(int denomination)
        {
            return ValidBillDenominations.Contains(denomination);
        }
    }
}

