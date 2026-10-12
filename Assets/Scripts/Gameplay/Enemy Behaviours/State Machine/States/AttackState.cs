namespace Gameplay.Enemies.States
{
    public class AttackState : IState
    {
        public Enemy Subject { get; set; }

        private readonly float _attackTime;
        public AttackState(float attackTime)
        {
            _attackTime = attackTime;
        }
        
        public void Enter()
        {
            
        }

        public void Update()
        {
            
        }

        public void Exit()
        {
        }
    }
}