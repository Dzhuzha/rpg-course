using System.Collections.Generic;
using GameDevTV.Utils;
using RPG.Core;
using RPG.Atributes;
using RPG.Movement;
using RPG.Saving;
using RPG.Stats;
using UnityEngine;

namespace RPG.Combat
{
    public class Fighter : ActionScheduler, IAction, ISaveable, IModifierProvider
    {
        [SerializeField] private Mover _mover;
        [SerializeField] private ActionScheduler _scheduler;
        [SerializeField] private Animator _animator;
        [SerializeField] private Transform _rightHandTransform = null;
        [SerializeField] private Transform _leftHandTransform = null;
        [SerializeField] private WeaponConfig _defaultWeapon = null;
        [SerializeField] private BaseStats _baseStats;

        //[SerializeField] private ProjectTile _arrowPrefab;
        private LazyValue<WeaponConfig> _currentWeapon;
        private RuntimeAnimatorController _defaultAnimatorController;
        private Health _targetHealth;
        private Transform _targetPosition;
        private float _timeSinceLastAttack = 0f;

        private void Awake()
        {
            _defaultAnimatorController = _animator.runtimeAnimatorController;
            _currentWeapon = new LazyValue<WeaponConfig>(SetupDefaultWeapon);
        }

        private void Start()
        {
            _currentWeapon.ForceInit();
        }

        private void Update()
        {
            ReduceAttackTime();

            if (_targetPosition == null) return;

            if (Vector3.Distance(transform.position, _targetPosition.position) > _currentWeapon.value.WeaponRange)
            {
                _mover.MoveTo(_targetPosition.position);
            }
            else
            {
                _mover.CancelAction();
                AttackBehaviour();
            }
        }

        private WeaponConfig SetupDefaultWeapon()
        {
            SpawnWeapon(_defaultWeapon);
            return _defaultWeapon;
        }

        public void EquipWeapon(WeaponConfig weaponConfig)
        {
            SpawnWeapon(weaponConfig);
            _currentWeapon.value = weaponConfig;
        }

        private void SpawnWeapon(WeaponConfig weaponToSpawn)
        {
            if (weaponToSpawn == null)
            {
                _currentWeapon.value = _defaultWeapon;
                return;
            }

            if (_leftHandTransform.GetComponentInChildren<Weapon>() != null)
            {
                Destroy(_leftHandTransform.GetComponentInChildren<Weapon>().gameObject);
            }

            if (_rightHandTransform.GetComponentInChildren<Weapon>() != null)
            {
                Destroy(_rightHandTransform.GetComponentInChildren<Weapon>().gameObject);
            }

            if (weaponToSpawn.Prefab != null)
            {
                Transform handTransform = weaponToSpawn.IsRightHanded ? _rightHandTransform : _leftHandTransform;
                Instantiate(weaponToSpawn.Prefab, handTransform);
                _currentWeapon.value = weaponToSpawn;
            }

            if (weaponToSpawn.AnimatorOverride != null)
            {
                _animator.runtimeAnimatorController = weaponToSpawn.AnimatorOverride;
            }
            else
            {
                _animator.runtimeAnimatorController = _defaultAnimatorController;
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
                _timeSinceLastAttack = _currentWeapon.value.TimeBetweenAttacks;
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

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return _currentWeapon.value.Damage;
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return _currentWeapon.value.BonusPercent;
            }
        }

        private void Shoot() //Animation event
        {
            if (_currentWeapon.value.ProjectTile == null) return;
            CreateProjectTile();
        }

        private void CreateProjectTile()
        {
            ProjectTile projectTile = Instantiate(_currentWeapon.value.ProjectTile, _leftHandTransform);
            projectTile.transform.SetParent(transform.root);
            // projectTile.InitArrow(_targetHealth, gameObject, _currentWeapon.Damage);
            projectTile.InitArrow(_targetHealth, gameObject, CalculateDamage());
        }

        private void Hit() //Animation event
        {
            if (_targetHealth == null)
            {
                return;
            }

            if (_currentWeapon.value.ProjectTile != null)
            {
                CreateProjectTile();
                return;
            }

            _targetHealth.TakeDamage(gameObject, CalculateDamage());
        }

        private float CalculateDamage()
        {
            return _baseStats.GetStat(Stat.Damage);
        }

        public object CaptureState()
        {
            return _currentWeapon.value.name;
        }

        public void RestoreState(object state)
        {
            string weaponName = (string)state;
            WeaponConfig weaponConfig = Resources.Load<WeaponConfig>(weaponName);
            EquipWeapon(weaponConfig);
        }
    }
}