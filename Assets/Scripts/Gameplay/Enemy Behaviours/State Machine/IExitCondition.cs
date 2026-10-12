namespace Gameplay.Enemies
{
    public interface IExitCondition
    {
        public Enemy Subject { set; }
        public bool ShouldExit();
    }
}