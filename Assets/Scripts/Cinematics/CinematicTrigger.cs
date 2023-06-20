using RPG.Control;
using RPG.Saving;
using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics
{
    public class CinematicTrigger : MonoBehaviour, ISaveable
    {
        private PlayableDirector _playableDirector;
        private bool _isAlreadyPlayed;

        private void Start()
        {
            _playableDirector = GetComponent<PlayableDirector>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out PlayerController player) && !_isAlreadyPlayed)
            {
                _playableDirector.Play();
                _isAlreadyPlayed = true;
            }
        }

        public object CaptureState()
        {
            return _isAlreadyPlayed;
        }

        public void RestoreState(object state)
        {
            _isAlreadyPlayed = (bool)state;
        }
    }
}