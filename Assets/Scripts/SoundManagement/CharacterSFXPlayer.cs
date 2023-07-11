using System.Collections.Generic;
using RPG.Atributes;
using UnityEngine;

namespace RPG.SoundManagement
{
    public class CharacterSFXPlayer : MonoBehaviour
    {
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private Health _health;
        [SerializeField] private List<AudioClip> _damagedAudioClips;
        [SerializeField] private List<AudioClip> _deadAudioClips;
        [SerializeField] private List<AudioClip> _healedAudioCLips;

        private void OnEnable()
        {
            _health.HealthChanged += PlayDamagedSFX;
            _health.Dead += PlayDeadSFX;
        }

        private void OnDisable()
        {
            _health.HealthChanged -= PlayDamagedSFX;
            _health.Dead -= PlayDeadSFX;
        }

        private void PlayDamagedSFX(float newHPValue, float fullHPValue)
        {
            if (_damagedAudioClips == null || _damagedAudioClips.Count == 0 || _audioSource.isPlaying) return;
            if (newHPValue < fullHPValue && newHPValue > 0)
            {
                _audioSource.PlayOneShot(_damagedAudioClips[Random.Range(0, _damagedAudioClips.Count)]);
            }
        }
        
        private void PlayDeadSFX()
        {
            if (_deadAudioClips == null || _deadAudioClips.Count == 0) return;

            _audioSource.PlayOneShot(_deadAudioClips[Random.Range(0, _deadAudioClips.Count)]);
        }

        private void PlayHealedSFX()
        {
            if (_healedAudioCLips == null || _healedAudioCLips.Count == 0) return;
            
        }
    }
}