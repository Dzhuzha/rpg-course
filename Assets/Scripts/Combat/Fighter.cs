using System;
using RPG.Core;
using RPG.Movement;
using UnityEngine;

namespace RPG.Combat
{
    public class Fighter : ActionScheduler, IAction
    {
        [SerializeField] private Mover _mover;
        [SerializeField] private float _weaponRange = 2f;
        [SerializeField] private ActionScheduler _scheduler;
        [SerializeField] private Animator _animator;
        [SerializeField] private float _meleeDamage;
        [SerializeField] private float _timeBetweenAttacks = 1f;
        [SerializeField] private GameObject _weaponPrefab = null;
        [SerializeField] private Transform _handTransform = null;
        
        private Health _targetHealth;
        private Transform _targetPosition;
        private float _timeSinceLastAttack = 0f;


        private void Start()
        {
            SpawnWeapon();
        }

        private void Update()
        {
            ReduceAttackTime();
            if (_targetPosition == null) return;

            if (Vector3.Distance(transform.position, _targetPosition.position) > _weaponRange)
            {
                _mover.MoveTo(_targetPosition.position);
            }
            else
            {
                _mover.CancelAction();
                AttackBehaviour();
            }
        }

        private void SpawnWeapon()
        {
            if (_weaponPrefab != null && _handTransform != null)
            {
                Instantiate(_weaponPrefab, _handTransform);
            }
        }

        private void ReduceAttackTime()
        {
            _timeSinceLastAttack = _timeSinceLastAttack > 0 ? _timeSinceLastAttack - Time.deltaTime : 0f;
        }

        private void AttackBehaviour()
        {
            transform.LookAt(_targetPosition);
            
            if (_timeSinceLastAttack <= 0f)
            {
                _timeSinceLastAttack = _timeBetweenAttacks;
                TriggerAttack();
            }
            
            if (_targetHealth.IsDead)
            {
                CancelAction();
            }
        }

        public void Attack(GameObject combatTarget)
        {
            _scheduler.StartAction(this);
            _targetPosition = combatTarget.transform;
        }

        public bool CanAttack(GameObject combatTarget)
        {
            _targetHealth = combatTarget.GetComponent<Health>();
            return _targetHealth.IsDead == false;
        }

        public void CancelAction()
        {
            StopAttack();
            _targetPosition = null;
            _mover.CancelAction();
        }

        private void TriggerAttack()
        {
            _animator.ResetTrigger("StopAttack");
            _animator.SetTrigger("Attack");
        }

        private void StopAttack()
        {
            _animator.ResetTrigger("Attack");
            _animator.SetTrigger("StopAttack");
        }

        private void Hit()
        {
            if (_targetHealth == null) { return; }
            
            _targetHealth.TakeDamage(_meleeDamage);
        }
    }
}