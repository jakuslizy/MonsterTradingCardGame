using MonsterTradingCardGame.Domain.Models;

namespace MonsterTradingCardGame.Data.Repositories;

public interface IPackageRepository
{
    void CreatePackage(Package package, List<Card> cards);
    Package? GetPackage(int userId);
}