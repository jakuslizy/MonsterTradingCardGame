using MonsterTradingCardGame.Data.Repositories;
using MonsterTradingCardGame.Domain.Models;
using System.Text.Json;

namespace MonsterTradingCardGame.Business.Services
{
    public class PackageService : IPackageService
    {
        private readonly PackageRepository _packageRepository;
        private readonly ICardService _cardService;

        public PackageService(PackageRepository packageRepository, 
                             ICardService cardService)
        {
            _packageRepository = packageRepository;
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
                var card = _cardService.CreateCard(dto.Id, dto.Name, 
                                                 (int)Math.Round(dto.Damage), 
                                                 ElementType.Normal);
                if (card == null)
                {
                    throw new ArgumentException($"Invalid card type: {dto.Name}");
                }
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

        private class CardDto
        {
            public CardDto(float damage)
            {
                Damage = damage;
            }

            public string Id { get; set; } = "";
            public string Name { get; } = "";
            public float Damage { get; }
        }
    }
}