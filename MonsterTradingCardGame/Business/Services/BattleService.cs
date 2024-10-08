using MonsterTradingCardGame.Domain.Models;
using MonsterTradingCardGame.Business.Logic;

namespace MonsterTradingCardGame.Business.Services;

public class BattleService
{
    private User Player1 { get; }
    private User Player2 { get; }
    private const int MaxRounds = 100;
    private List<string> BattleLog { get; } = new List<string>();
    private BattleLogic _battleLogic;

    public BattleService(User player1, User player2)
    {
        Player1 = player1;
        Player2 = player2;
        _battleLogic = new BattleLogic();
    }

    public string ExecuteBattle()
    {
        var player1Deck = new List<Card>(Player1.Deck);
        var player2Deck = new List<Card>(Player2.Deck);
        var random = new Random();

        for (int round = 1; round <= MaxRounds; round++)
        {
            if (!player1Deck.Any() || !player2Deck.Any())
                break;

            var card1 = player1Deck[random.Next(player1Deck.Count)];
            var card2 = player2Deck[random.Next(player2Deck.Count)];

            BattleLog.Add($"Runde {round}: {Player1.Username}'s {card1.Name} gegen {Player2.Username}'s {card2.Name}");

            var winner = _battleLogic.DetermineRoundWinner(card1, card2);

            if (winner == 1)
            {
                player2Deck.Remove(card2);
                player1Deck.Add(card2);
                BattleLog.Add($"{Player1.Username} gewinnt die Runde!");
            }
            else if (winner == 2)
            {
                player1Deck.Remove(card1);
                player2Deck.Add(card1);
                BattleLog.Add($"{Player2.Username} gewinnt die Runde!");
            }
            else
            {
                BattleLog.Add("Unentschieden in dieser Runde!");
            }
        }

        UpdatePlayerStats(player1Deck.Count, player2Deck.Count);
        return string.Join("\n", BattleLog);
    }


    private void UpdatePlayerStats(int player1CardCount, int player2CardCount)
    {
        if (player1CardCount > player2CardCount)
        {
            Player1.UpdateElo(3);
            Player2.UpdateElo(-5);
            BattleLog.Add($"{Player1.Username} gewinnt den Kampf!");
        }
        else if (player2CardCount > player1CardCount)
        {
            Player2.UpdateElo(3);
            Player1.UpdateElo(-5);
            BattleLog.Add($"{Player2.Username} gewinnt den Kampf!");
        }
        else
        {
            BattleLog.Add("Der Kampf endet unentschieden!");
        }
    }
}