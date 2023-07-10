﻿using RPG.Atributes;
using RPG.Control;
using UnityEngine;

namespace RPG.Combat
{
    [RequireComponent(typeof(Atributes.Health))]
    public class CombatTarget : MonoBehaviour, IRaycastable
    {
        public bool HandleRaycast(PlayerController callingController)
        {
            if (!callingController.transform.TryGetComponent(out Fighter fighter)) return false;

            if (Input.GetMouseButton(0) && fighter.CanAttack(gameObject))
            {
                callingController.StartTrackingTargetHealth(GetComponent<Health>());
                fighter.Attack(gameObject);
            }
            
            return true;
        }

        public CursorType GetCursorType()
        {
            return CursorType.Combat;
        }
    }
}