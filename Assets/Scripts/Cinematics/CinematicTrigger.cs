using RPG.Control;
using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics
{
    public class CinematicTrigger : MonoBehaviour
    {
        private PlayableDirector _playableDirector;
        private bool isAlreadyPlayed;
        
        private void Start()
        {
            _playableDirector = GetComponent<PlayableDirector>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out PlayerController player) && !isAlreadyPlayed)
            {
                _playableDirector.Play();
                isAlreadyPlayed = true;
            }
        }
    }
}