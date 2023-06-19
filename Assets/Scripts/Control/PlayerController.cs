using RPG.Core;
using RPG.Combat;
using RPG.Movement;
using UnityEngine;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private Mover _mover;
        [SerializeField] private Fighter _fighter;

        private Health _attackerHealth;

        private void Start()
        {
            _attackerHealth = GetComponent<Health>();
        }
        
        private void Update()
        {
            if (_attackerHealth.IsDead) return;
            if (InteractWithCombat()) return;
            if (InteractWithMovement()) return;

            Debug.Log("Cannot to move or attack!");
        }

        private bool InteractWithCombat()
        {
            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());

            foreach (var hit in hits)
            {
                if (!hit.transform.TryGetComponent(out CombatTarget target)) continue;

                if (Input.GetMouseButton(0) && _fighter.CanAttack(target.gameObject))
                {
                    _fighter.Attack(target.gameObject);
                }

                return true;
            }

            return false;
        }

        private bool InteractWithMovement()
        {
            if (Physics.Raycast(GetMouseRay(), out RaycastHit hit))
            {
                if (Input.GetMouseButton(0))
                {
                    _mover.StartMoveAction(hit.point);
                }

                return true;
            }

            return false;
        }

        private Ray GetMouseRay()
        {
            return _camera.ScreenPointToRay(Input.mousePosition);
        }
    }
}