using Newtonsoft.Json.Linq;
using RPG.Control;
using RPG.Saving;
using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics
{
    public class CinematicTrigger : MonoBehaviour, IJsonSaveable
    {
        private PlayableDirector _playableDirector;
        private bool _isAlreadyPlayed;

        private void Awake()
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

        public JToken CaptureAsJToken()
        {
            return JToken.FromObject(_isAlreadyPlayed);
        }

        public void RestoreFromJToken(JToken state)
        {
            _isAlreadyPlayed = state.ToObject<bool>();
        }
    }
}