namespace Gameplay.Enemies
{
    public interface IExitCondition
    {
        public Enemy Subject { set; }
        public void StateEntered();
        public bool ShouldExit();
    }
}