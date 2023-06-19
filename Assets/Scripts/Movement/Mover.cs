using RPG.Core;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Movement
{
    public class Mover : ActionScheduler, IAction
    {
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private Animator _animator;
        [SerializeField] private ActionScheduler _scheduler;
        
        private Health _health;
        public float MovementSpeed { get; private set; }

        private void Start()
        {
            _health = GetComponent<Health>();
            MovementSpeed = _agent.speed;
        }

        private void Update()
        {
            _agent.enabled = _health.IsDead == false;
            PlayMovingAnimation();
        }

        private void PlayMovingAnimation()
        {
            Vector3 velocity = transform.InverseTransformDirection(_agent.velocity);
            _animator.SetFloat("MovingSpeed", velocity.z);
            //SetFloat("Locomotion", _agent.velocity.magnitude);
        }

        public void MoveTo(Vector3 destination)
        {
            _agent.destination = destination;
        }

        public void StartMoveAction(Vector3 destination)
        {
            _scheduler.StartAction(this);
            MoveTo(destination);
        }

        private void StopMoving()
        {
            _agent.SetDestination(transform.position);
        }

        public void CancelAction()
        {
            StopMoving();
        }

        public void SetSpeed(float speed)
        {
            _agent.speed = speed;
        }
    }
}