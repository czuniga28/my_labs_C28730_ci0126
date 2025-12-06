using ExamTwo.Services;

namespace ExamTwo.Repositories
{
    public class ChangeRepository : IChangeRepository
    {
        private readonly Dictionary<int, int> _coins;

        public ChangeRepository()
        {
            _coins = new Dictionary<int, int>
            {
                { 500, 20 },
                { 100, 30 },
                { 50, 50 },
                { 25, 25 }
            };
            ValidateCoinDenominations();
        }

        private void ValidateCoinDenominations()
        {
            foreach (var denomination in _coins.Keys)
            {
                if (!PaymentValidator.IsValidCoinDenomination(denomination))
                {
                    throw new InvalidOperationException($"Invalid coin denomination {denomination} in ChangeRepository. Must be one of: {string.Join(", ", PaymentValidator.ValidCoinDenominations)}");
                }
            }
        }

        public Dictionary<int, int> GetAvailableCoins()
        {
            return new Dictionary<int, int>(_coins);
        }

        public bool CanMakeChange(int amount)
        {
            if (amount <= 0)
                return true;

            var tempCoins = new Dictionary<int, int>(_coins);
            var remaining = amount;

            foreach (var coinValue in tempCoins.Keys.OrderByDescending(k => k))
            {
                var needed = remaining / coinValue;
                var available = tempCoins[coinValue];
                var used = Math.Min(needed, available);
                remaining -= used * coinValue;
            }

            return remaining == 0;
        }

        public Dictionary<int, int> CalculateChangeBreakdown(int amount)
        {
            var breakdown = new Dictionary<int, int>();
            var tempCoins = new Dictionary<int, int>(_coins);
            var remaining = amount;

            foreach (var coinValue in tempCoins.Keys.OrderByDescending(k => k))
            {
                var needed = remaining / coinValue;
                var available = tempCoins[coinValue];
                var used = Math.Min(needed, available);
                
                if (used > 0)
                {
                    breakdown[coinValue] = used;
                    remaining -= used * coinValue;
                }
            }

            return breakdown;
        }

        public bool DeductCoins(Dictionary<int, int> coinsToDeduct)
        {
            foreach (var (denomination, quantity) in coinsToDeduct)
            {
                if (!PaymentValidator.IsValidCoinDenomination(denomination))
                {
                    return false;
                }
            }

            foreach (var (denomination, quantity) in coinsToDeduct)
            {
                if (!_coins.ContainsKey(denomination) || _coins[denomination] < quantity)
                    return false;
            }

            foreach (var (denomination, quantity) in coinsToDeduct)
            {
                _coins[denomination] -= quantity;
            }

            return true;
        }
    }
}

