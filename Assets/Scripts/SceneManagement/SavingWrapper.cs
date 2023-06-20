using RPG.Saving;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class SavingWrapper : MonoBehaviour
    {
        private const string DEFAULT_SAVE_FILE_NAME = "data";
        
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

        private void Save()
        {
            // call saving system to save
            GetComponent<SavingSystem>().Save(DEFAULT_SAVE_FILE_NAME);
        }

        private void Load()
        {
            // call saving system to load
            GetComponent<SavingSystem>().Load(DEFAULT_SAVE_FILE_NAME);
        }
    }
}