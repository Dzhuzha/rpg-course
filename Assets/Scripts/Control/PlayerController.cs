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

            if (InteractWithComponent()) return;
            if (InteractWithMovement()) return;
            
            SetCursor(CursorType.None);
        }

        private bool InteractWithComponent()
        {
            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());

            foreach (RaycastHit hit in hits)
            {
                IRaycastable[] raycastableList = hit.transform.GetComponents<IRaycastable>();
                foreach (IRaycastable raycastable in raycastableList)
                {
                    if (raycastable.HandleRaycast(this))
                    {
                        SetCursor(raycastable.GetCursorType());
                        return true;
                    }
                }
            }

            return false;
        }

        private bool InteractWithUI()
        {
            if (!EventSystem.current.IsPointerOverGameObject()) return false;
            
            SetCursor(CursorType.UI);
            return true;
        }

        public void StartTrackingTargetHealth(Health targetHealth)
        {
            _enemyHealthDisplay.InitTarget(targetHealth);
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
    }
}