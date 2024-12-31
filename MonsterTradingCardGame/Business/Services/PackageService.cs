using MonsterTradingCardGame.Domain.Models;
using System.Text.Json;
using MonsterTradingCardGame.Business.Services.Interfaces;
using MonsterTradingCardGame.Data.Repositories.Interfaces;

namespace MonsterTradingCardGame.Business.Services
{
    public class PackageService(IPackageRepository packageRepository, ICardService cardService)
        : IPackageService
    {
        public void CreatePackage(string cardDtosJson, string username)
        {
            if (username != "admin")
            {
                throw new UnauthorizedAccessException("Only admin can create packages");
            }

            // Direkt in eine Liste von Dictionaries deserialisieren
            var cardsData = JsonSerializer.Deserialize<List<Dictionary<string, JsonElement>>>(cardDtosJson)
                            ?? throw new ArgumentException("Invalid card data");

            var cards = new List<Card>();
            foreach (var cardData in cardsData)
            {
                var id = cardData["Id"].GetString() ?? "";
                var name = cardData["Name"].GetString() ?? "";
                var damage = (int)Math.Round(cardData["Damage"].GetDouble());

                var card = cardService.CreateCard(id, name, damage, ElementType.Normal);
                if (card == null)
                {
                    throw new ArgumentException($"Invalid card type: {name}");
                }

                cards.Add(card);
            }

            var package = new Package();
            foreach (var card in cards)
            {
                package.AddCard(card);
            }

            packageRepository.CreatePackage(package, cards);
        }
    }
}