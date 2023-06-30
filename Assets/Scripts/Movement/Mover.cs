using System;
using RPG.Core;
using RPG.Atributes;
using UnityEngine;
using UnityEngine.AI;
using RPG.Saving;

namespace RPG.Movement
{
    public class Mover : ActionScheduler, IAction, ISaveable
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

        public object CaptureState()
        {
            MoverSaveData moverSaveData = new MoverSaveData(new SerializableVector3(transform.position),
                new SerializableVector3(transform.eulerAngles));
            return moverSaveData as object;
        }

        public void RestoreState(object state)
        {
            MoverSaveData data = (MoverSaveData) state;
            _agent.enabled = false;
            transform.position = data.Position.ToVector();
            transform.eulerAngles = data.Rotation.ToVector();
            _agent.enabled = true;
        }

        [Serializable]
        struct MoverSaveData
        {
            public SerializableVector3 Position { get; private set; }
            public SerializableVector3 Rotation { get; private set; }

            public MoverSaveData(SerializableVector3 position, SerializableVector3 rotation)
            {
                Position = position;
                Rotation = rotation;
            }
        }
    }
}