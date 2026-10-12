namespace Gameplay.Enemies
{
    public interface IState
    {
        public Enemy Subject { get; set; }
        public void Enter();
        public void Update();
        public void Exit();
    }
}