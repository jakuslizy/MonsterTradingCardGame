using MonsterTradingCardGame.Business.Logic;
using MonsterTradingCardGame.Domain.Models;
using System.Text;
using System.Data;
using MonsterTradingCardGame.Business.Services.Interfaces;
using MonsterTradingCardGame.Data;
using MonsterTradingCardGame.Data.Repositories.Interfaces;

namespace MonsterTradingCardGame.Business.Services;

public class BattleService(IStatsRepository statsRepository, IUserRepository userRepository)
    : IBattleService
{
    private readonly BattleLogic _battleLogic = new();
    private readonly Random _random = new();

    public string ExecuteBattle(User player1, User player2)
    {
        // Prüfen ob es verschiedene Spieler sind
        if (player1.Id == player2.Id)
        {
            throw new InvalidOperationException("Cannot battle against yourself");
        }

        // Lade die aktuellen Decks aus der Datenbank
        var player1Deck = userRepository.GetUserDeck(player1.Id);
        var player2Deck = userRepository.GetUserDeck(player2.Id);

        // Validierung der Decks
        if (player1Deck.Count != 4 || player2Deck.Count != 4)
        {
            throw new InvalidOperationException(
                $"Both players must have exactly 4 cards in their deck. Player1: {player1Deck.Count}, Player2: {player2Deck.Count}");
        }

        var log = new StringBuilder();
        var rounds = 0;

        log.AppendLine($"Battle: {player1.Username} vs {player2.Username}\n");

        while (rounds < 100 && player1Deck.Count > 0 && player2Deck.Count > 0)
        {
            rounds++;
            var card1 = player1Deck[_random.Next(player1Deck.Count)];
            var card2 = player2Deck[_random.Next(player2Deck.Count)];

            log.AppendLine($"Round {rounds}:");
            log.AppendLine($"{player1.Username}'s {card1.Name} ({card1.ElementType}, {card1.Damage} Damage) vs");
            log.AppendLine($"{player2.Username}'s {card2.Name} ({card2.ElementType}, {card2.Damage} Damage)");

            var winner = _battleLogic.DetermineRoundWinner(card1, card2);

            switch (winner)
            {
                case 1:
                    log.AppendLine($"{player1.Username} wins round {rounds}\n");
                    TransferCard(card2, player2.Id, player1.Id);
                    player2Deck.Remove(card2);
                    player1Deck.Add(card2);
                    break;
                case 2:
                    log.AppendLine($"{player2.Username} wins round {rounds}\n");
                    TransferCard(card1, player1.Id, player2.Id);
                    player1Deck.Remove(card1);
                    player2Deck.Add(card1);
                    break;
                default:
                    log.AppendLine("Round ended in a draw\n");
                    break;
            }
        }

        // Bestimme Gesamtsieger
        string battleResult;
        if (player1Deck.Count > player2Deck.Count)
        {
            battleResult = $"{player1.Username} wins the battle!";
            UpdateStats(player1, player2, false);
        }
        else if (player2Deck.Count > player1Deck.Count)
        {
            battleResult = $"{player2.Username} wins the battle!";
            UpdateStats(player2, player1, false);
        }
        else
        {
            battleResult = "Battle ended in a draw!";
            UpdateStats(player1, player2, true);
        }

        log.AppendLine(battleResult);
        log.AppendLine(
            $"Final Score - {player1.Username}: {player1Deck.Count} cards, {player2.Username}: {player2Deck.Count} cards");

        return log.ToString();
    }

    private void TransferCard(Card card, int fromUserId, int toUserId)
    {
        using var connection = DataLayer.Instance.CreateConnection();
        using var transaction = connection.BeginTransaction();
        try
        {
            using var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = @"
                UPDATE cards 
                SET user_id = @toUserId,
                    in_deck = true
                WHERE id = @cardId 
                AND user_id = @fromUserId";

            DataLayer.AddParameterWithValue(command, "@cardId", DbType.String, card.Id);
            DataLayer.AddParameterWithValue(command, "@fromUserId", DbType.Int32, fromUserId);
            DataLayer.AddParameterWithValue(command, "@toUserId", DbType.Int32, toUserId);

            var rowsAffected = command.ExecuteNonQuery();
            if (rowsAffected == 0)
            {
                throw new InvalidOperationException($"Card {card.Id} could not be transferred");
            }

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
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