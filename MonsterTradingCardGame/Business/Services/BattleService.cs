using MonsterTradingCardGame.Business.Logic;
using MonsterTradingCardGame.Domain.Models;
using System.Text;

namespace MonsterTradingCardGame.Business.Services;

public class BattleService : IBattleService
{
    private readonly BattleLogic _battleLogic;

    public BattleService()
    {
        _battleLogic = new BattleLogic();
    }

    public string ExecuteBattle(User player1, User player2)
    {
        var log = new StringBuilder();
        var rounds = 0;
        var player1Deck = new List<Card>(player1.Deck);
        var player2Deck = new List<Card>(player2.Deck);

        while (rounds < 100 && player1Deck.Count > 0 && player2Deck.Count > 0)
        {
            rounds++;
            var card1 = player1Deck[new Random().Next(player1Deck.Count)];
            var card2 = player2Deck[new Random().Next(player2Deck.Count)];

            log.AppendLine($"Round {rounds}: {player1.Username}'s {card1} vs {player2.Username}'s {card2}");

            var winner = _battleLogic.DetermineRoundWinner(card1, card2);
            switch (winner)
            {
                case 1:
                    log.AppendLine($"{player1.Username} wins the round");
                    player2Deck.Remove(card2);
                    player1Deck.Add(card2);
                    break;
                case 2:
                    log.AppendLine($"{player2.Username} wins the round");
                    player1Deck.Remove(card1);
                    player2Deck.Add(card1);
                    break;
                default:
                    log.AppendLine("It's a draw");
                    break;
            }
        }

        if (player1Deck.Count > player2Deck.Count)
        {
            log.AppendLine($"{player1.Username} wins the battle!");
            player1.UpdateElo(3);
            player2.UpdateElo(-5);
        }
        else if (player2Deck.Count > player1Deck.Count)
        {
            log.AppendLine($"{player2.Username} wins the battle!");
            player2.UpdateElo(3);
            player1.UpdateElo(-5);
        }
        else
        {
            log.AppendLine("The battle ends in a draw!");
        }

        return log.ToString();
    }
}