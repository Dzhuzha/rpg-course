using RPG.Combat;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private WeaponConfig _equipedWeapon;
    
    public WeaponConfig EquipedWeapon => _equipedWeapon;
}
