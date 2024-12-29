using MonsterTradingCardGame.API.Server.DTOs;
using MonsterTradingCardGame.Business.Services;
using MonsterTradingCardGame.Data.Repositories;
using MonsterTradingCardGame.Domain.Models;

namespace MonsterTradingCardGame.API.Server.Handlers
{
    public class PackageHandler
    {
        private readonly IPackageService _packageService;
        private readonly IUserRepository _userRepository;
        private readonly IPackageRepository _packageRepository;

        public PackageHandler(IPackageService packageService, IUserRepository userRepository, IPackageRepository packageRepository)
        {
            _packageService = packageService;
            _userRepository = userRepository;
            _packageRepository = packageRepository;
        }

        public Response HandleCreatePackage(string username, string body)
        {
            try
            {
                _packageService.CreatePackage(body, username);
                return new Response(201, "Package created successfully", "application/json");
            }
            catch (UnauthorizedAccessException)
            {
                return new Response(403, "Forbidden - only admin can create packages", "application/json");
            }
            catch (ArgumentException ex)
            {
                return new Response(400, ex.Message, "application/json");
            }
            catch (Exception)
            {
                return new Response(500, "Internal server error", "application/json");
            }
        }

        public Response HandleBuyPackage(User user)
        {
            try 
            {
                // Hole aktuelle Coins aus der DB
                var currentCoins = _userRepository.GetUserCoins(user.Id);
                if (currentCoins < Package.PackagePrice)
                {
                    return new Response(402, "Not enough money", "application/json");
                }

                var package = _packageRepository.GetPackage(user.Id);
                if (package == null)
                {
                    return new Response(404, "No packages available", "application/json");
                }

                // Direkt in der DB aktualisieren
                _userRepository.UpdateUserCoins(user.Id, currentCoins - Package.PackagePrice);
                _userRepository.SaveUserCards(user.Id, package.GetCards().ToList());

                return new Response(201, "Package successfully acquired", "application/json");
            }
            catch (Exception ex)
            {
                return new Response(500, $"Error while acquiring package: {ex.Message}", "application/json");
            }
        }
    }
}
        