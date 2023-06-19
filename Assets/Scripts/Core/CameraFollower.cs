using UnityEngine;
using RPG.Movement;

namespace RPG.Core
{
    public class CameraFollower : MonoBehaviour
    {
        [SerializeField] private Mover _player;
        [SerializeField] private Vector3 _cameraOffset;

        private void LateUpdate()
        {
            transform.position = _player.transform.position + _cameraOffset;
        }
    }
}