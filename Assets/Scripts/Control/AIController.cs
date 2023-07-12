using System;
using GameDevTV.Utils;
using RPG.Atributes;
using RPG.Combat;
using RPG.Movement;
using UnityEngine;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] private float _chaseDistance = 5f;
        [SerializeField] private float _suspiciousTimer = 3f;
        [SerializeField] private PatrolPath _patrolPath;
        [SerializeField] private float _aggrevationTime = 5f;

        private Fighter _fighter;
        private Mover _mover;
        private PlayerController _player;
        private Health _health;
        private LazyValue<Vector3> _startPosition;
        private float _timeAfterTargetLost = 0;
        private float _waypointTolerance = 1f;
        private int _currentWaypointIndex;
        private float _timeAfterAggrevated = 0;

        private void Awake()
        {
            _fighter = GetComponent<Fighter>();
            _player = FindObjectOfType<PlayerController>();
            _health = GetComponent<Health>();
            _mover = GetComponent<Mover>();
            _startPosition = new LazyValue<Vector3>(GetStartPosition);
       //     _chaseDistance = _fighter.GetAttackDistance() > _chaseDistance ? _fighter.GetAttackDistance() : _chaseDistance;
        }

        private void OnEnable()
        {
            _health.Attacked += Aggrevate;
        }

        private void OnDisable()
        {
            _health.Attacked -= Aggrevate;
        }

        private Vector3 GetStartPosition()
        {
            return transform.position;
        }

        private void Aggrevate()
        {
            _timeAfterAggrevated = _aggrevationTime;
        }

        private void Update()
        {
            if (_health.IsDead) return;

            ExecuteAttackLogic();
            UpdateTimer();
        }

        private bool IsAggrevated()
        {   
            float distanceToPlayer = Vector3.Distance(transform.position, _player.transform.position);

            return distanceToPlayer < _chaseDistance || _timeAfterAggrevated <= 0;
        }

        private void ExecuteAttackLogic()
        {
            if (IsAggrevated() && _fighter.CanAttack(_player.gameObject))
            {
                AttackBehaviour();
            }
            else if (_timeAfterTargetLost > 0)
            {
                SuspicionBehaviour();
            }
            else
            {
                PatrolBehaviour();
            }
        }

        private void UpdateTimer()
        {
          //  if (_timeAfterAggrevated < 0) return;
            
            _timeAfterAggrevated -= Time.deltaTime;
        }

        private void AttackBehaviour()
        {
            _mover.SetSpeed(_mover.MovementSpeed);
            _timeAfterTargetLost = _suspiciousTimer;
            _fighter.Attack(_player.gameObject);
        }

        private void SuspicionBehaviour()
        {
            _fighter.CancelAction();
            _timeAfterTargetLost -= Time.deltaTime;
        }

        private void PatrolBehaviour()
        {
            Vector3 nextPosition = _startPosition.value;
            _mover.SetSpeed(_mover.MovementSpeed / 2);

            if (_patrolPath != null)
            {
                if (AtWaypoint())
                {
                    _timeAfterTargetLost = _suspiciousTimer;
                    CyclePosition();
                }

                nextPosition = GetCurrentWaypoint();
            }

            _mover.StartMoveAction(nextPosition);
        }

        private Vector3 GetCurrentWaypoint()
        {
            return _patrolPath.GetPosition(_currentWaypointIndex);
        }

        private void CyclePosition()
        {
            _currentWaypointIndex =  _patrolPath.GetNextIndex(_currentWaypointIndex);
        }

        private bool AtWaypoint()
        {
            float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
            return distanceToWaypoint < _waypointTolerance;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, _chaseDistance);
        }
    }
}