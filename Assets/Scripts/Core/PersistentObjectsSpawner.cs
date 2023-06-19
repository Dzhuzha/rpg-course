using UnityEngine;

namespace RPG.Core
{
    public class PersistentObjectsSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject _persistentGameobjectPrefab;

        private static bool _hasSpawned = false;
        
        private void Awake()
        {
            if (_hasSpawned) return;

            SpawnPersistentObjects();
        }

        private void SpawnPersistentObjects()
        {
            DontDestroyOnLoad(Instantiate(_persistentGameobjectPrefab));
            _hasSpawned = true;
        }
    }
}