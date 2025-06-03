using Gameplay.Data;
using UnityEngine;
using UnityEngine.AI;

namespace Gameplay.Enemies
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class SimpleEnemy : Enemy
    {
        [SerializeField] private NavMeshAgent _agent;

        private void Start()
        {
            
        }

        private void Update()
        {
            _agent.SetDestination(Target.transform.position);
        }

        public override void Validate()
        {
            base.Validate();
            _agent = GetComponent<NavMeshAgent>();
        }
    }
}
