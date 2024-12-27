namespace MonsterTradingCardGame.Business.Factories;
using Domain.Models;
using Domain.Models.MonsterCards;

public static class CardFactory
{
    public static Card? CreateCard(string id, string name, int damage, ElementType elementType)
    {
        // Element aus dem Namen extrahieren
        if (name.StartsWith("Water")) elementType = ElementType.Water;
        if (name.StartsWith("Fire")) elementType = ElementType.Fire;
        
        // Kartentyp aus dem Namen extrahieren
        if (name.EndsWith("Spell"))
        {
            return new SpellCard(id, name, damage, elementType);
        }
        
        // Monster-Karten
        return name switch
        {
            var n when n.EndsWith("Goblin") => new Goblin(id, name, damage, elementType),
            var n when n.EndsWith("Dragon") => new Dragon(id, name, damage, elementType),
            var n when n.EndsWith("Wizard") => new Wizzard(id, name, damage, elementType),
            var n when n.EndsWith("Ork") => new Ork(id, name, damage, elementType),
            var n when n.EndsWith("Knight") => new Knight(id, name, damage, elementType),
            var n when n.EndsWith("Kraken") => new Kraken(id, name, damage, elementType),
            var n when n.EndsWith("FireElf") => new FireElf(id, name, damage, elementType),
            _ => null
        };
    }
} 