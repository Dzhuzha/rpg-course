using UnityEngine;

namespace RPG.Combat
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private WeaponConfig _equippedWeapon;
        [SerializeField] private AudioSource _audioSource;

        public WeaponConfig EquippedWeapon => _equippedWeapon;
        
        public void TriggerHit()
        {
            _audioSource.Play();
        }
    }
}