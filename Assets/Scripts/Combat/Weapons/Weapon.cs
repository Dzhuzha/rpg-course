using System;
using UnityEngine;

namespace RPG.Combat
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private WeaponConfig _equippedWeapon;

        public WeaponConfig EquippedWeapon => _equippedWeapon;

        public event Action HitDealed;

        public void TriggerHit()
        {
            Debug.Log("!!!!!!!Hit!!!!!!" + gameObject.name);
        }
    }
}