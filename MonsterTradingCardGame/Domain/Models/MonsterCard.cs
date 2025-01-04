namespace MonsterTradingCardGame.Domain.Models;

public abstract class MonsterCard(string id, string name, int damage, ElementType elementType)
    : Card(id, name, damage, elementType);