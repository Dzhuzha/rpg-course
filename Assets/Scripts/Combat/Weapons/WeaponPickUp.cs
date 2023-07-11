using System.Collections;
using RPG.Combat;
using RPG.Control;
using UnityEngine;

public class WeaponPickUp : MonoBehaviour, IRaycastable
{
    [SerializeField] private WeaponConfig _config;
    [SerializeField] private float _healthToRestore = 0f;
    [SerializeField] private float _respawnTime = 5f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out RPG.Control.PlayerController player))
        {
            PickUp(player.gameObject);
        }
    }

    private void PickUp(GameObject owner)
    {
        if (_config != null) 
        {
            owner.GetComponent<Fighter>().EquipWeapon(_config);
        }

        if (_healthToRestore > 0)
        {
            owner.GetComponent<RPG.Atributes.Health>().Heal(_healthToRestore);
        }

        StartCoroutine(HideForSeconds(_respawnTime));
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

    public bool HandleRaycast(PlayerController callingController)
    {
        if (Input.GetMouseButtonDown(0))
        {
            PickUp(callingController.gameObject);
        }

        return true;
    }

    public CursorType GetCursorType()
    {
        return CursorType.Pickup;
    }
}