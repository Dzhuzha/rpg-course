using System;
using RPG.Atributes;
using RPG.Combat;
using RPG.Movement;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private Mover _mover;
        [SerializeField] private Fighter _fighter;
        [SerializeField] private EnemyHealthDisplay _enemyHealthDisplay;
        [NonReorderable, SerializeField] private CursorMapping[] _cursorMappings;
        
        private Health _attackerHealth;

        [Serializable]
        struct CursorMapping
        {
            public CursorType Type;
            public Vector2 Hotspot;
            public Texture2D Texture;
        }
        
        private void Awake()
        {
            _attackerHealth = GetComponent<Health>();
        }
        
        private void Update()
        {
            if (InteractWithUI())return;

            if (_attackerHealth.IsDead)
            {
                SetCursor(CursorType.None);
                return;
            }
            
            if (InteractWithCombat()) return;
            if (InteractWithMovement()) return;
            
            SetCursor(CursorType.None);
        }

        private bool InteractWithUI()
        {
            if (!EventSystem.current.IsPointerOverGameObject()) return false;
            
            SetCursor(CursorType.UI);
            return true;
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
                    _enemyHealthDisplay.InitTarget(target.GetComponent<Health>());
                }

                SetCursor(CursorType.Combat);
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
                    _enemyHealthDisplay.ResetTarget();
                }

                SetCursor(CursorType.Movement);
                return true;
            }

            return false;
        }
        
        private CursorMapping GetCursorMapping(CursorType cursorType)
        {
            foreach (var mapping in _cursorMappings)
            {
                if (mapping.Type == cursorType)
                {
                    return mapping;
                }
            }

            return _cursorMappings[0];
        }

        private void SetCursor(CursorType cursorType)
        {
            CursorMapping mapping = GetCursorMapping(cursorType);
            Cursor.SetCursor(mapping.Texture, mapping.Hotspot, CursorMode.Auto);
        }

        private Ray GetMouseRay()
        {
            return _camera.ScreenPointToRay(Input.mousePosition);
        }
        
        private enum CursorType
        {
            None,
            Combat,
            Movement,
            UI
        }
    }
}