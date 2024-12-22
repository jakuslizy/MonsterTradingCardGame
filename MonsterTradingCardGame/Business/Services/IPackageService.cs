namespace MonsterTradingCardGame.Business.Services;

public interface IPackageService
{
    void CreatePackage(string cardsJson, string username);
}