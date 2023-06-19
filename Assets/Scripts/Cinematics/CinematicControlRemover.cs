using RPG.Control;
using RPG.Core;
using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics
{
    public class CinematicControlRemover : MonoBehaviour
    {
        private PlayableDirector _playableDirector;
        private PlayerController _playerController;
        private ActionScheduler _playerActionScheduler;

        private void Start()
        {
            _playableDirector = GetComponent<PlayableDirector>();
            _playerController = FindObjectOfType<PlayerController>();
            _playerActionScheduler = _playerController.GetComponent<ActionScheduler>();
            
            Subscribe();
        }

        private void OnDestroy()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            _playableDirector.played += DisableControl;
            _playableDirector.stopped += EnableControl;
        }

        private void Unsubscribe()
        {
            _playableDirector.played -= DisableControl;
            _playableDirector.stopped -= EnableControl;
        }

        private void DisableControl(PlayableDirector playableDirector)
        {
            _playerController.enabled = false;
            _playerActionScheduler.CancelCurrentAction();
            _playerActionScheduler.IsDisabled = true;
        }

        private void EnableControl(PlayableDirector playableDirector)
        {
            _playerActionScheduler.IsDisabled = false;
            _playerController.enabled = true;
        }
    }
}