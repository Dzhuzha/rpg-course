using UnityEngine;

namespace RPG.Control
{
    public class PatrolPath : MonoBehaviour
    {
        private const float _waitpointGizmoRadius = 0.3f;

        private void OnDrawGizmos()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Vector3 startPosition = GetPosition(i);
                Vector3 finishPosition = GetPosition(GetNextIndex(i));

                Gizmos.DrawSphere(startPosition, _waitpointGizmoRadius);
                Gizmos.DrawLine(startPosition, finishPosition);
            }
        }

        public Vector3 GetPosition(int index)
        {
            return transform.GetChild(index).position;
        }

        public int GetNextIndex(int index)
        {
            int newIndex = index + 1;
            return newIndex < transform.childCount ? newIndex : 0;
        }
    }
}