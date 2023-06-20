using System.Collections;
using RPG.Saving;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class SavingWrapper : MonoBehaviour
    {
        [SerializeField] private float _fadeInTime = 2f;
        
        private const string DEFAULT_SAVE_FILE_NAME = "data";
        private SavingSystem _savingSystem;
        private Fader _fader;

        private IEnumerator Start()
        {
            if (_savingSystem == null)
            {
                _savingSystem = GetComponent<SavingSystem>();
            }
            
            if (_fader == null)
            {
                _fader = FindObjectOfType<Fader>();
            }

            _fader.FadeOutImmediate();
            yield return _savingSystem.LoadLastScene(DEFAULT_SAVE_FILE_NAME);
            yield return _fader.FadeIn(_fadeInTime);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                Load();
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                Save();
            }
        }

        public void Save()
        {
            // call saving system to save
            _savingSystem.Save(DEFAULT_SAVE_FILE_NAME);
        }

        public void Load()
        {
            // call saving system to load
            _savingSystem.Load(DEFAULT_SAVE_FILE_NAME);
        }
    }
}