using RPG.Combat;
using UnityEngine;

public class WeaponPickUp : MonoBehaviour
{
    [SerializeField] private WeaponConfig _config;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out RPG.Control.PlayerController player))
        {
            player.GetComponent<Fighter>().EquipWeapon(_config);
            Destroy(gameObject);
        }
    }
}