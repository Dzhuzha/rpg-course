using RPG.Inventory;
using UnityEngine;

namespace RPG.Combat
{
  [CreateAssetMenu(order = 51, fileName = "NewWeapon", menuName = "Weapons/Create New Weapon")]
  public class WeaponConfig : EquipableItem
  {
    [SerializeField] private Weapon _weaponPrefab;
    [SerializeField] private AnimatorOverrideController _animatorOverride = null;
    [SerializeField] private float _weaponRange = 2f;
    [SerializeField] private float _damage = 5f;
    [SerializeField] private float _bonusPercent = 0f;
    [SerializeField] private float _timeBetweenAttacks = 0.8f;
    [SerializeField] private bool _isRightHanded = true;
    [SerializeField] private ProjectTile _projectTile = null;
    [SerializeField] private AudioClip _creationAudioClip = null;
    [SerializeField] private AudioClip _usageAudioClip = null;

    public Weapon Prefab => _weaponPrefab;
    public AnimatorOverrideController AnimatorOverride => _animatorOverride;
    public float TimeBetweenAttacks => _timeBetweenAttacks;
    public float Damage => _damage;
    public float BonusPercent => _bonusPercent;
    public float WeaponRange => _weaponRange;
    public bool IsRightHanded => _isRightHanded;
    public ProjectTile ProjectTile => _projectTile;
    public AudioClip UsageAudioClip => _usageAudioClip;
    public AudioClip CreationAudioClip => _creationAudioClip;
  }
}