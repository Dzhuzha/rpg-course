using System;
using RPG.Atributes;
using RPG.Combat;
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
        [SerializeField] private float _maxNavMeshPathLength = 5f;
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
            if (!IsMovable(out Vector3 targetPosition)) return false;
            
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
            
            NavMeshPath path = new NavMeshPath();
            bool hasPath = NavMesh.CalculatePath(transform.position, targetPosition, NavMesh.AllAreas, path);
            if (!hasPath || path.status != NavMeshPathStatus.PathComplete) return false;
            if (GetPathLength(path) > _maxNavMeshPathLength) return false;

            return hasCastToNavMesh;
        }

        private float GetPathLength(NavMeshPath path)
        {
            float totalDistance = 0f;
            if (path.corners.Length < 2) return totalDistance;

            for (int i = 1; i < path.corners.Length; i++)
            {
                totalDistance += Vector3.Distance(path.corners[i-1], path.corners[i]);
            }
            
            return totalDistance;
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