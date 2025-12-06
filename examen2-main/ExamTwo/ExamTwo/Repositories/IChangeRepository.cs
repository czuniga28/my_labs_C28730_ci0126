namespace ExamTwo.Repositories
{
    public interface IChangeRepository
    {
        Dictionary<int, int> GetAvailableCoins();
        bool CanMakeChange(int amount);
        bool DeductCoins(Dictionary<int, int> coinsToDeduct);
        Dictionary<int, int> CalculateChangeBreakdown(int amount);
    }
}

