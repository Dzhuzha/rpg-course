﻿using RPG.Core;
using RPG.Movement;
using UnityEngine;

namespace RPG.Combat
{
    public class Fighter : ActionScheduler, IAction
    {
        [SerializeField] private Mover _mover;
        [SerializeField] private ActionScheduler _scheduler;
        [SerializeField] private Animator _animator;
        [SerializeField] private Transform _rightHandTransform = null;
        [SerializeField] private Transform _leftHandTransform = null;
        [SerializeField] private WeaponConfig _defaultWeapon = null;
     //   [SerializeField] private ProjectTile _arrowPrefab;

        private WeaponConfig _currentWeapon;
        
        private Health _targetHealth;
        private Transform _targetPosition;
        private float _timeSinceLastAttack = 0f;

        private void Start()
        {
            EquipWeapon(_defaultWeapon);
        }

        private void Update()
        {
            ReduceAttackTime();
            
            if (_targetPosition == null) return;

            if (Vector3.Distance(transform.position, _targetPosition.position) > _currentWeapon.WeaponRange)
            {
                _mover.MoveTo(_targetPosition.position);
            }
            else
            {
                _mover.CancelAction();
                AttackBehaviour();
            }
        }

        public void EquipWeapon(WeaponConfig weaponConfig)
        {
            SpawnWeapon(weaponConfig);
            _currentWeapon = weaponConfig;
        }

        private void UseDefaultWeapon()
        {
            _currentWeapon = _defaultWeapon;
        }

        private void SpawnWeapon(WeaponConfig weaponToSpawn)
        {
            if (weaponToSpawn == null)
            {
                UseDefaultWeapon();
                return;
            }

            if (weaponToSpawn.Prefab != null)
            {
                if (_currentWeapon != null)
                {
                    if (_leftHandTransform.GetComponentInChildren<Weapon>() != null)
                    {
                        Destroy(_leftHandTransform.GetComponentInChildren<Weapon>().gameObject);
                    }
                    if (_rightHandTransform.GetComponentInChildren<Weapon>() != null)
                    {
                        Destroy(_rightHandTransform.GetComponentInChildren<Weapon>().gameObject);
                    }
                }
                
                Transform handTransform = weaponToSpawn.IsRightHanded ? _rightHandTransform : _leftHandTransform;
                Instantiate(weaponToSpawn.Prefab, handTransform);
                _currentWeapon = weaponToSpawn;
            }

            if (weaponToSpawn.AnimatorOverride != null)
            {
                _animator.runtimeAnimatorController = weaponToSpawn.AnimatorOverride;
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
                _timeSinceLastAttack = _currentWeapon.TimeBetweenAttacks;
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

        public float GetAttackDistance()
        {
            return _defaultWeapon.WeaponRange;
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

        private void Shoot() //Animation event
        {
            if (_currentWeapon.ProjectTile == null) return;

            ProjectTile arrow = Instantiate(_currentWeapon.ProjectTile, _leftHandTransform);
            arrow.transform.SetParent(transform.root);
            arrow.InitArrow(_targetHealth, _currentWeapon.Damage);
        }

        private void Hit() //Animation event
        {
            if (_targetHealth == null) { return; }
            
            _targetHealth.TakeDamage(_currentWeapon.Damage);
        }
    }
}