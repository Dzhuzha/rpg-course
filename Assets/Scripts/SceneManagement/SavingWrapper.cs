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

        private void Awake()
        {
            StartCoroutine(LoadLastScene());
        }

        private IEnumerator LoadLastScene()
        {
            _savingSystem = _savingSystem == null ? GetComponent<SavingSystem>() : _savingSystem;
            yield return _savingSystem.LoadLastScene(DEFAULT_SAVE_FILE_NAME);
            _fader = _fader == null ? FindObjectOfType<Fader>() : _fader;
            _fader.FadeOutImmediate();
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

            if (Input.GetKeyDown(KeyCode.D))
            {
                Delete();
            }
        }

        public void Save()
        {
            _savingSystem.Save(DEFAULT_SAVE_FILE_NAME);
        }

        public void Load()
        {
            _savingSystem.Load(DEFAULT_SAVE_FILE_NAME);
        }

        private void Delete()
        {
            _savingSystem.Delete(DEFAULT_SAVE_FILE_NAME);
            Debug.Log("Save file Deleted!");
        }
    }
}