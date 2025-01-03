using MonsterTradingCardGame.Domain.Models;

namespace MonsterTradingCardGame.Data.Repositories.Interfaces;

public interface IPackageRepository
{
    void CreatePackage(Package package, List<Card> cards);
    Package? GetPackage(int userId);
    void UpdatePackageOwner(int packageId, int userId);
}