using RPG.Atributes;
using RPG.Control;
using UnityEngine;

namespace RPG.Combat
{
    [RequireComponent(typeof(Atributes.Health))]
    public class CombatTarget : MonoBehaviour, IRaycastable
    {
        private Fighter _fighterComponent;
        
        private void Awake()
        {
            _fighterComponent = GetComponent<Fighter>();
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if (!callingController.transform.TryGetComponent(out Fighter fighter)) return false;
            if (!_fighterComponent.enabled) return false;

            if (Input.GetMouseButton(0) && fighter.CanAttack(gameObject))
            {
                callingController.StartTrackingTargetHealth(GetComponent<Health>());
                fighter.Attack(gameObject);
            }
            
            return true;
        }

        public CursorType GetCursorType()
        {
            if (!_fighterComponent.enabled) return CursorType.None;
  
            return CursorType.Combat;
        }
    }
}