using MonsterTradingCardGame.Data.Repositories;
using MonsterTradingCardGame.Domain.Models;
using MonsterTradingCardGame.Domain.Models.MonsterCards;
using System.Text.Json;

namespace MonsterTradingCardGame.Business.Services
{
    public interface IPackageService
    {
        void CreatePackage(string cardsJson, string username);
    }

    public class PackageService : IPackageService
    {
        private readonly PackageRepository _packageRepository;
        private readonly UserRepository _userRepository;

        public PackageService(PackageRepository packageRepository, UserRepository userRepository)
        {
            _packageRepository = packageRepository;
            _userRepository = userRepository;
        }

        public void CreatePackage(string cardsJson, string username)
        {
            // Verify admin rights
            if (username != "admin")
            {
                throw new UnauthorizedAccessException("Only admin can create packages");
            }

            // Parse cards from JSON
            var cardDtos = JsonSerializer.Deserialize<List<CardDto>>(cardsJson) 
                ?? throw new ArgumentException("Invalid card data");

            if (cardDtos.Count != 5)
            {
                throw new ArgumentException("Package must contain exactly 5 cards");
            }

            // Convert DTOs to domain models
            var cards = new List<Card>();
            foreach (var dto in cardDtos)
            {
                var elementType = DetermineElementType(dto.Name);
                Card card = CreateCard(dto.Id, dto.Name, (int)Math.Round(dto.Damage), elementType);
                cards.Add(card);
            }

            // Create package with cards
            var package = new Package();
            foreach (var card in cards)
            {
                package.AddCard(card);
            }

            _packageRepository.CreatePackage(package, cards);
        }

        private Card CreateCard(string id, string name, int damage, ElementType elementType)
        {
            // Spell-Karten
            if (name.Contains("Spell"))
            {
                return new SpellCard(id, name, damage, elementType);
            }

            // Monster-Karten
            return name switch
            {
                var n when n.Contains("Dragon") => new Dragon(id, name, damage, elementType),
                var n when n.Contains("FireElf") => new FireElf(id, name, damage, elementType),
                var n when n.Contains("Kraken") => new Kraken(id, name, damage, elementType),
                var n when n.Contains("Knight") => new Knight(id, name, damage, elementType),
                var n when n.Contains("Wizard") => new Wizzard(id, name, damage, elementType),
                var n when n.Contains("Ork") => new Ork(id, name, damage, elementType),
                var n when n.Contains("Goblin") => new Goblin(id, name, damage, elementType),
                // Defaultfall - wenn kein spezifischer Typ erkannt wird, erstellen wir einen Dragon als Fallback
                _ => new Dragon(id, name, damage, elementType)
            };
        }

        private ElementType DetermineElementType(string cardName)
        {
            if (cardName.Contains("Water", StringComparison.OrdinalIgnoreCase))
                return ElementType.Water;
            if (cardName.Contains("Fire", StringComparison.OrdinalIgnoreCase))
                return ElementType.Fire;
            return ElementType.Normal;
        }

        private class CardDto
        {
            public string Id { get; set; } = "";
            public string Name { get; set; } = "";
            public float Damage { get; set; }
        }
    }
}