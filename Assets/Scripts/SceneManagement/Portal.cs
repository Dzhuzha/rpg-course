using System.Collections;
using RPG.Control;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace RPG.SceneManagement
{
    public class Portal : MonoBehaviour
    {
        [SerializeField] private sbyte _sceneToLoadIndex = -1;
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private DestinationIdentifier _destination;
        [SerializeField] private float _fadeOutTime;
        [SerializeField] private float _fadeInTime;

        public Fader Fader { get; private set; }
        public DestinationIdentifier Destination => _destination;

        private void Awake()
        {
            Fader = FindObjectOfType<Fader>();
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out PlayerController player))
            {
                if (_sceneToLoadIndex < 0)
                {
                    Debug.Log("Scene to load not set!");
                    return;
                }
                
                StartCoroutine(Transition());
            }
        }

        private IEnumerator Transition()
        {
            DontDestroyOnLoad(gameObject);

            yield return Fader.FadeOut(_fadeOutTime);
            yield return SceneManager.LoadSceneAsync(_sceneToLoadIndex);
            
            Portal exitPortal = GetOtherPortals();
            UpdatePlayer(exitPortal);
            yield return exitPortal.Fader.FadeIn(_fadeInTime);

            Destroy(gameObject);
        }

        private void UpdatePlayer(Portal exitPortal)
        {
            PlayerController player = FindObjectOfType<PlayerController>();
            player.GetComponent<NavMeshAgent>().Warp(exitPortal._spawnPoint.position);
            player.transform.rotation = exitPortal._spawnPoint.rotation;
        }

        private Portal GetOtherPortals()
        {
            Portal[] portals = FindObjectsOfType<Portal>();

            foreach (var portal in portals)
            {
                if (portal == this || portal.Destination != _destination) continue;

                return portal;
            }

            return null;
        }

        private void OnDrawGizmos()
        {
            Debug.DrawRay(_spawnPoint.position, _spawnPoint.forward, Color.red);
        }
    }

    public enum DestinationIdentifier
    {
        A,
        B, 
        C,
        D,
        E,
        F
    }
}