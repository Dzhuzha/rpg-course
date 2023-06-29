using System.Collections;
using RPG.Combat;
using UnityEngine;

public class WeaponPickUp : MonoBehaviour
{
    [SerializeField] private WeaponConfig _config;
    [SerializeField] private float _respawnTime = 5f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out RPG.Control.PlayerController player))
        {
            player.GetComponent<Fighter>().EquipWeapon(_config);
            StartCoroutine(HideForSeconds(_respawnTime));
        }
    }

    private IEnumerator HideForSeconds(float seconds)
    {
        ShowPickUp(false);
        yield return new WaitForSeconds(seconds);
        ShowPickUp(true);
    }

    private void ShowPickUp(bool show)
    {
        GetComponent<Collider>().enabled = show;
        
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(show);
        }
    }
}