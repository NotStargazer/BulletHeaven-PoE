namespace Gameplay.Data
{
    public struct ProjectileSnapshot
    {
        public readonly int Count;
        public readonly float Speed;
        public readonly float Range;
        public readonly int Pierce;

        public ProjectileSnapshot(int count, float speed, float range, int pierce)
        {
            Count = count;
            Speed = speed;
            Range = range;
            Pierce = pierce;
        }
    }
}