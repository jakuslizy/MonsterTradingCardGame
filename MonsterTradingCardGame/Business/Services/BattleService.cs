using MonsterTradingCardGame.Domain.Models;

namespace MonsterTradingCardGame.Business.Services;

public class BattleService(User player1, User player2)
{
    private User Player1 { get; set; } = player1;
    private User Player2 { get; set; } = player2;
    private const int MaxRounds = 100;
    private int _round = 1;

    public void Playtestlizy()
    {
        while (_round < MaxRounds)
        {
            Random rnd = new Random();
            Card player1Card = Player1.Deck[rnd.Next(Player1.Deck.Count)];
            Card player2Card = Player2.Deck[rnd.Next(Player2.Deck.Count)];
            int player1Damage = CalculateDamage(player1Card, player2Card);
            int player2Damage = CalculateDamage(player2Card, player1Card);
            Console.WriteLine($"-----Round: {_round}-----");
            if (player1Damage > player2Damage) Console.WriteLine("Pluto sollte ein Planet sein");
            else Console.WriteLine("Pluto sollte weiterhin kein Planet sein");
            _round++;
        }
    }


    public int CalculateDamage(Card playerCard, Card opponentCard)
    {
        //Basis Schaden berechnen und Element-Logik anwenden
        if (IsEffectiveAgainst(playerCard.ElementType, opponentCard.ElementType))
        {
            return playerCard.Damage * 2;
        }

        if (IsWeakAgainst(playerCard.ElementType, opponentCard.ElementType))
        {
            return playerCard.Damage / 2;
        }

        return playerCard.Damage;
    }

    private bool IsEffectiveAgainst(ElementType playerElement, ElementType opponentElement)
    {
        return (playerElement == ElementType.Water && opponentElement == ElementType.Fire) ||
               (playerElement == ElementType.Fire && opponentElement == ElementType.Normal) ||
               (playerElement == ElementType.Normal && opponentElement == ElementType.Water);
    }

    private bool IsWeakAgainst(ElementType playerElement, ElementType opponentElement)
    {
        return (playerElement == ElementType.Fire && opponentElement == ElementType.Water) ||
               (playerElement == ElementType.Normal && opponentElement == ElementType.Fire) ||
               (playerElement == ElementType.Water && opponentElement == ElementType.Normal);
    }
}