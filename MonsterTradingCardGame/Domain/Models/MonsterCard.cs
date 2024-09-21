namespace MonsterTradingCardGame.Domain.Models;

public class MonsterCard(string name, int damage, ElementType elementType) : Card(name, damage, elementType);