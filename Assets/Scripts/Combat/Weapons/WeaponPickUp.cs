using System.Collections;
using RPG.Combat;
using RPG.Control;
using UnityEngine;

public class WeaponPickUp : MonoBehaviour, IRaycastable
{
    [SerializeField] private WeaponConfig _config;
    [SerializeField] private float _respawnTime = 5f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out RPG.Control.PlayerController player))
        {
            PickUp(player.GetComponent<Fighter>());
        }
    }

    private void PickUp(Fighter owner)
    {
        owner.EquipWeapon(_config);
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
            PickUp(callingController.GetComponent<Fighter>());
        }

        return true;
    }

    public CursorType GetCursorType()
    {
        return CursorType.Pickup;
    }
}