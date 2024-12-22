using MonsterTradingCardGame.Data.Repositories;
using MonsterTradingCardGame.Domain.Models;
using MonsterTradingCardGame.Domain.Models.MonsterCards;
using System.Text.Json;

namespace MonsterTradingCardGame.Business.Services
{
    public class PackageService : IPackageService
    {
        private readonly PackageRepository _packageRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICardService _cardService;

        public PackageService(PackageRepository packageRepository, 
                             IUserRepository userRepository,
                             ICardService cardService)
        {
            _packageRepository = packageRepository;
            _userRepository = userRepository;
            _cardService = cardService;
        }

        public void CreatePackage(string cardDtosJson, string username)
        {
            // Verify admin rights
            if (username != "admin")
            {
                throw new UnauthorizedAccessException("Only admin can create packages");
            }

            // Parse cards from JSON
            var cardDtos = JsonSerializer.Deserialize<List<CardDto>>(cardDtosJson) 
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
                Card card = _cardService.CreateCard(dto.Id, dto.Name, 
                                                  (int)Math.Round(dto.Damage), 
                                                  elementType);
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