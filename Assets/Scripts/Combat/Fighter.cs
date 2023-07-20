using System;
using System.Collections.Generic;
using GameDevTV.Utils;
using RPG.Core;
using RPG.Atributes;
using RPG.Inventory;
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
        [SerializeField] private Equipment _equipment;
        
        //[SerializeField] private ProjectTile _arrowPrefab;
        private const string ATTACK_TRIGGER = "Attack";
        private const string STOP_ATTACKING = "StopAttack";
        private WeaponConfig _currentWeaponConfig;
        private LazyValue<Weapon> _currentWeapon;
        private RuntimeAnimatorController _defaultAnimatorController;
        private Health _targetHealth;
        private Transform _targetPosition;
        private float _timeSinceLastAttack = 0f;

        private void Awake()
        {
            _defaultAnimatorController = _animator.runtimeAnimatorController;
            _currentWeaponConfig = _defaultWeapon;
            _currentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);
        }

        private void OnEnable()
        {
            if (_equipment == null) return;
            _equipment.EquipmentUpdated += UpdateWeapon;
        }

        private void Start()
        {
            _currentWeapon.ForceInit();
        }

        private void OnDisable()
        {
            if (_equipment == null) return;
            _equipment.EquipmentUpdated -= UpdateWeapon;
        }

        private void Update()
        {
            ReduceAttackTime();

            if (_targetPosition == null) return;

            if (Vector3.Distance(transform.position, _targetPosition.position) > _currentWeaponConfig.WeaponRange)
            {
                _mover.MoveTo(_targetPosition.position);
            }
            else
            {
                _mover.CancelAction();
                AttackBehaviour();
            }
        }

        private void UpdateWeapon()
        {
            EquipableItem weaponToEquip = _equipment.GetItemInSlot(EquipLocation.MainHand); 
            var weaponConfig = weaponToEquip as WeaponConfig;
           
            if (weaponConfig == null)
            {
                EquipWeapon(_defaultWeapon);
                return;
            }

            EquipWeapon(weaponConfig);
        }

        private Weapon SetupDefaultWeapon()
        {
            return SpawnWeapon(_defaultWeapon);
        }

        public void EquipWeapon(WeaponConfig weaponConfig)
        {
             _currentWeapon.value = SpawnWeapon(weaponConfig);
            _currentWeaponConfig = weaponConfig;
        }

        private Weapon SpawnWeapon(WeaponConfig weaponToSpawn)
        {
            Weapon spawnedWeapon = null;
            
            if (weaponToSpawn == null)
            {
                _currentWeaponConfig = _defaultWeapon;
             
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
                spawnedWeapon = Instantiate(weaponToSpawn.Prefab, handTransform);
                _currentWeaponConfig = weaponToSpawn;
            }

            _animator.runtimeAnimatorController = weaponToSpawn.AnimatorOverride == null ? _defaultAnimatorController : weaponToSpawn.AnimatorOverride;
            return spawnedWeapon;
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
                _timeSinceLastAttack = _currentWeaponConfig.TimeBetweenAttacks;
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
            if (!_mover.CanMoveTo(combatTarget.transform.position)) return false;

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
            _animator.ResetTrigger(STOP_ATTACKING);
            _animator.SetTrigger(ATTACK_TRIGGER);
        }

        private void StopAttack()
        {
            _animator.ResetTrigger(ATTACK_TRIGGER);
            _animator.SetTrigger(STOP_ATTACKING);
        }

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return _currentWeaponConfig.Damage;
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return _currentWeaponConfig.BonusPercent;
            }
        }

        private void Shoot() //Animation event
        {
            if (_currentWeaponConfig.ProjectTile == null) return;
            CreateProjectTile();
        }

        private void CreateProjectTile()
        {
            ProjectTile projectTile = Instantiate(_currentWeaponConfig.ProjectTile, _leftHandTransform);
            projectTile.transform.SetParent(transform.root);
            // projectTile.InitArrow(_targetHealth, gameObject, _currentWeapon.Damage);
            projectTile.InitArrow(_targetHealth, gameObject, CalculateDamage(), _currentWeaponConfig.CreationAudioClip, _currentWeaponConfig.UsageAudioClip);
        }

        private void Hit() //Animation event
        {
            if (_targetHealth == null)
            {
                return;
            }

            if (_currentWeapon.value != null)
            {
                _currentWeapon.value.TriggerHit();
            }

            if (_currentWeaponConfig.ProjectTile != null)
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
            return _currentWeaponConfig.name;
        }

        public void RestoreState(object state)
        {
            string weaponName = (string)state;
            WeaponConfig weaponConfig = Resources.Load<WeaponConfig>(weaponName);
            EquipWeapon(weaponConfig);
        }
    }
}