using MonsterTradingCardGame.Business.Logic;
using MonsterTradingCardGame.Domain.Models;
using System.Text;
using System.Data;
using MonsterTradingCardGame.Business.Services.Interfaces;
using MonsterTradingCardGame.Data;
using MonsterTradingCardGame.Data.Repositories.Interfaces;

namespace MonsterTradingCardGame.Business.Services;

public class BattleService(IStatsRepository statsRepository, IUserRepository userRepository, ICardRepository cardRepository)
    : IBattleService
{
    private readonly BattleLogic _battleLogic = new();
    private readonly Random _random = new();
    private readonly ICardRepository _cardRepository = cardRepository;

    public string ExecuteBattle(User player1, User player2)
    {
        // Prüfen, ob es verschiedene Spieler sind
        if (player1.Id == player2.Id)
        {
            throw new InvalidOperationException("Cannot battle against yourself");
        }

        // Hole die User-Daten für die Display-Namen
        var player1Data = userRepository.GetUserById(player1.Id);
        var player2Data = userRepository.GetUserById(player2.Id);

        var player1DisplayName = string.IsNullOrEmpty(player1Data?.Name) ? player1.Username : player1Data.Name;
        var player2DisplayName = string.IsNullOrEmpty(player2Data?.Name) ? player2.Username : player2Data.Name;

        var player1Deck = userRepository.GetUserDeck(player1.Id);
        var player2Deck = userRepository.GetUserDeck(player2.Id);

        if (player1Deck.Count != 4 || player2Deck.Count != 4)
        {
            throw new InvalidOperationException($"Both players must have exactly 4 cards in their deck");
        }

        var log = new StringBuilder();
        var rounds = 0;

        log.AppendLine($"Battle: {player1DisplayName} vs {player2DisplayName}\n");

        while (rounds < 100 && player1Deck.Count > 0 && player2Deck.Count > 0)
        {
            rounds++;
            var card1 = player1Deck[_random.Next(player1Deck.Count)];
            var card2 = player2Deck[_random.Next(player2Deck.Count)];

            log.AppendLine($"Round {rounds}:");

            // Berechne Schaden und prüfe auf kritische Treffer
            var damage1 = _battleLogic.CalculateDamage(card1, card2);
            var damage2 = _battleLogic.CalculateDamage(card2, card1);

            // Zeige die Basis-Karten und deren tatsächlichen Schaden
            log.AppendLine(
                $"{player1DisplayName}'s {card1.Name} ({card1.ElementType}, Base Damage: {card1.Damage}, Effective Damage: {damage1}{(damage1 > card1.Damage ? " [CRITICAL HIT!]" : "")}) vs");
            log.AppendLine(
                $"{player2DisplayName}'s {card2.Name} ({card2.ElementType}, Base Damage: {card2.Damage}, Effective Damage: {damage2}{(damage2 > card2.Damage ? " [CRITICAL HIT!]" : "")})");

            var winner = _battleLogic.DetermineRoundWinner(card1, card2);

            switch (winner)
            {
                case 1:
                    log.AppendLine($"{player1DisplayName} wins round {rounds}\n");
                    _cardRepository.TransferCard(card2.Id, player2.Id, player1.Id);
                    player2Deck.Remove(card2);
                    player1Deck.Add(card2);
                    break;
                case 2:
                    log.AppendLine($"{player2DisplayName} wins round {rounds}\n");
                    _cardRepository.TransferCard(card1.Id, player1.Id, player2.Id);
                    player1Deck.Remove(card1);
                    player2Deck.Add(card1);
                    break;
                default:
                    log.AppendLine($"Round {rounds} ended in a draw\n");
                    break;
            }
        }

        string battleResult;
        if (player1Deck.Count > player2Deck.Count)
        {
            battleResult = $"{player1DisplayName} wins the battle!";
            UpdateStats(player1, player2, false);
        }
        else if (player2Deck.Count > player1Deck.Count)
        {
            battleResult = $"{player2DisplayName} wins the battle!";
            UpdateStats(player2, player1, false);
        }
        else
        {
            battleResult = "Battle ended in a draw!";
            UpdateStats(player1, player2, true);
        }

        log.AppendLine(battleResult);
        log.AppendLine(
            $"Final Score - {player1DisplayName}: {player1Deck.Count} cards, {player2DisplayName}: {player2Deck.Count} cards");

        return log.ToString();
    }

    private void UpdateStats(User winner, User loser, bool isDraw)
    {
        var winnerStats = statsRepository.GetStatsByUserId(winner.Id);
        var loserStats = statsRepository.GetStatsByUserId(loser.Id);

        if (winnerStats == null || loserStats == null)
        {
            throw new InvalidOperationException("Stats not found for one or both players");
        }

        if (!isDraw)
        {
            // ELO Berechnung
            winnerStats.Elo += 3;
            loserStats.Elo = Math.Max(0, loserStats.Elo - 5);

            winnerStats.GamesWon++;
            loserStats.GamesLost++;
        }

        // Spiele immer aktualisieren
        winnerStats.GamesPlayed++;
        loserStats.GamesPlayed++;

        statsRepository.UpdateStats(winnerStats);
        statsRepository.UpdateStats(loserStats);
    }
}