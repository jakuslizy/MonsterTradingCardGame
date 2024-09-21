namespace MonsterTradingCardGame.Domain.Models;

public class SpellCard(string name, int damage, ElementType elementType) : Card(name, damage, elementType);