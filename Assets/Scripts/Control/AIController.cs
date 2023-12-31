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

        private Fighter _fighter;
        private Mover _mover;
        private PlayerController _player;
        private Health _attackerHealth;
        private LazyValue<Vector3> _startPosition;
        private float _timeAfterTargetLost = 0;
        private float _waypointTolerance = 1f;
        private int _currentWaypointIndex;

        private void Awake()
        {
            _fighter = GetComponent<Fighter>();
            _player = FindObjectOfType<PlayerController>();
            _attackerHealth = GetComponent<Health>();
            _mover = GetComponent<Mover>();
            _startPosition = new LazyValue<Vector3>(GetStartPosition);
            
            _chaseDistance = _fighter.GetAttackDistance() > _chaseDistance ? _fighter.GetAttackDistance() : _chaseDistance;
        }

        private Vector3 GetStartPosition()
        {
            return transform.position;
        }

        private void Update()
        {
            if (_attackerHealth.IsDead || _fighter == null) return;

            ExecuteAttackLogic();
        }

        private bool CheckChasingDistance()
        {
            bool isInAttackRange = Vector3.Distance(transform.position, _player.transform.position) < _chaseDistance;
            return isInAttackRange;
        }

        private void ExecuteAttackLogic()
        {
            if (CheckChasingDistance() && _fighter.CanAttack(_player.gameObject))
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