using System;
using System.Collections.Generic;
using RPG.Atributes;
using RPG.Combat;
using RPG.Inventory;
using RPG.Movement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private Mover _mover;
        [SerializeField] private Fighter _fighter;
        [SerializeField] private float _maxNavMeshProjectionDistance = 1f;
        [SerializeField] private EnemyHealthDisplay _enemyHealthDisplay;
        [NonReorderable, SerializeField] private CursorMapping[] _cursorMappings;
        [SerializeField] private ActionStore _actionStore;
        
        private Health _attackerHealth;
        private bool _isDraggingUI;

        private List<KeyCode> _specialAbilityKeys = new List<KeyCode>
        {
            KeyCode.Alpha1,
            KeyCode.Alpha2,
            KeyCode.Alpha3,
            KeyCode.Alpha4,
            KeyCode.Alpha5,
            KeyCode.Alpha6
        };

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
            CheckSpecialAbilityKeys();
            
            if (InteractWithUI()) return;
            if (_isDraggingUI) return;
            
            if (_attackerHealth.IsDead)
            {
                SetCursor(CursorType.None);
                return;
            }

            if (InteractWithComponent()) return;
            if (InteractWithMovement()) return;
            
            SetCursor(CursorType.None);
        }

        private void CheckSpecialAbilityKeys()
        {
            if (_isDraggingUI) return;

            foreach (var keyCode in _specialAbilityKeys)
            {
                if (Input.GetKeyDown(keyCode))
                {
                    _actionStore.Use(_specialAbilityKeys.IndexOf(keyCode), gameObject);
                }
            }
        }

        private bool InteractWithComponent()
        {
            RaycastHit[] hits = RaycastSorted();

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

        private RaycastHit[] RaycastSorted()
        {
            RaycastHit[] unsortedHits = Physics.RaycastAll(GetMouseRay());
            float[] distances = new float [unsortedHits.Length];
            
            for (int i = 0; i < unsortedHits.Length; i++)
            {
                distances[i] = unsortedHits[i].distance;
            }
            
            Array.Sort(distances, unsortedHits);
            return unsortedHits;
        }

        private bool InteractWithUI()
        {
            if (Input.GetMouseButtonUp(0))
            {
                _isDraggingUI = false;
            }

            if (EventSystem.current.IsPointerOverGameObject())
            {
                if (Input.GetMouseButtonDown(0))
                {
                    _isDraggingUI = true;
                }
                SetCursor(CursorType.UI);
                return true;
            }
            
            return false;
        }

        public void StartTrackingTargetHealth(Health targetHealth)
        {
            _enemyHealthDisplay.InitTarget(targetHealth);
        }

        private bool InteractWithMovement()
        {
            if (!IsMovable(out Vector3 targetPosition)) return false;
            if (!_mover.CanMoveTo(targetPosition)) return false;

            if (Input.GetMouseButton(0))
            {
                _mover.StartMoveAction(targetPosition);
                _enemyHealthDisplay.ResetTarget();
            }
            
            SetCursor(CursorType.Movement);
            return true;
        }

        private bool IsMovable(out Vector3 targetPosition)
        {     
            targetPosition = new Vector3();
            if (!Physics.Raycast(GetMouseRay(), out RaycastHit hit)) return false;
            
            bool hasCastToNavMesh = NavMesh.SamplePosition(hit.point, out NavMeshHit navMeshHit, 
                _maxNavMeshProjectionDistance, NavMesh.AllAreas);

            if (hasCastToNavMesh)
            {
                targetPosition = navMeshHit.position;
            }

            return hasCastToNavMesh;
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