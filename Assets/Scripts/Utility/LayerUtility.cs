namespace Utility
{
    public enum DefinedLayers
    {
        Targetable = Enemy | Destructible,
        
        Player = 1 << 6,
        Enemy = 1 << 7,
        Destructible = 1 << 8,
    }
}
