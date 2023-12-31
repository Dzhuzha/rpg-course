using System.Collections;
using RPG.SceneManagement;
using UnityEngine;

namespace RPG.UI
{
    public class PauseMenu : MonoBehaviour
    {
        private SavingWrapper _savingWrapper;
        
        private void Awake()
        {
            _savingWrapper = FindObjectOfType<SavingWrapper>();
        }

        private void OnEnable()
        {
            Time.timeScale = 0f;
        }

        private void OnDisable()
        {
            Time.timeScale = 1f;
        }

        public void Load()
        {
            _savingWrapper.Load();
        }
        
        public void Save()
        {
            _savingWrapper.Save();
        }

        public void QuitAndSave()
        {
            StartCoroutine(SavingBeforeQuit());
        }

        public void DeleteSave()
        {
            _savingWrapper.Delete();
        }

        public void QuitWithoutSave()
        {
            Application.Quit();  
        }

        public IEnumerator SavingBeforeQuit()
        {
            Save();
            Time.timeScale = 1f;
            yield return new WaitForSeconds(1f);
            Application.Quit();
        }
    }
}