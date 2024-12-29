namespace MonsterTradingCardGame.Business.Services.Interfaces;

public interface IPackageService
{
    void CreatePackage(string cardsJson, string username);
}