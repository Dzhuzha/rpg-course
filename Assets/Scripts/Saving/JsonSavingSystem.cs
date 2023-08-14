using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG.Saving
{
    public class JsonSavingSystem : MonoBehaviour
    {
        private const string SAVE_FILE_EXTENSION = ".json";
        private const string BUILD_INDEX_LABEL = "lastSceneBuildIndex";

       // private IDictionary<string, JToken> _state;
        
        public IEnumerator LoadLastScene(string saveFile)
        {
          //  _state ??= LoadJsonFromFile(saveFile);
            JObject state = LoadJsonFromFile(saveFile);
            IDictionary<string, JToken> stateDict = state;
            int buildIndex = SceneManager.GetActiveScene().buildIndex;
            
            if (state.ContainsKey(BUILD_INDEX_LABEL))
            {
                buildIndex = (int) state[BUILD_INDEX_LABEL];
            }

            yield return SceneManager.LoadSceneAsync(buildIndex);
            RestoreFromToken(state);
        }

        public void Save(string saveFile)
        {
            JObject state = LoadJsonFromFile(saveFile);
            CaptureAsToken(state);
            SaveFileAsJSon(saveFile, state);
        }

        public void Delete(string saveFile)
        {
            File.Delete(GetPathFromSaveFile(saveFile));
        }

        public void Load(string saveFile)
        {
            RestoreFromToken(LoadJsonFromFile(saveFile));
        }

        public IEnumerable<string> ListSaves()
        {
            foreach (string path in Directory.EnumerateFiles(Application.persistentDataPath))
            {
                if (Path.GetExtension(path) == SAVE_FILE_EXTENSION)
                {
                    yield return Path.GetFileNameWithoutExtension(path);
                }
            }
        }

        private void SaveFileAsJSon(string saveFile, JObject state)
        {
            string path = GetPathFromSaveFile(saveFile);

            Debug.Log($"Saving to {path}");

            using (StreamWriter textWriter = File.CreateText(path))
            {
                using (JsonTextWriter writer = new JsonTextWriter(textWriter))
                {
                    writer.Formatting = Formatting.Indented;
                    state.WriteTo(writer);
                }
            }
        }

        private JObject LoadJsonFromFile(string saveFile)
        {
            string path = GetPathFromSaveFile(saveFile);

            if (!File.Exists(path))
            {
                return new JObject();
            }

            using (StreamReader textReader = File.OpenText(path))
            {
                using (JsonTextReader reader = new JsonTextReader(textReader))
                {
                    reader.FloatParseHandling = FloatParseHandling.Double;
                    return JObject.Load(reader);
                }
            }
        }

        private void CaptureAsToken(JObject state)
        {
            IDictionary<string, JToken> stateDictionary = state;

            foreach (JsonSaveableEntity saveableEntity in FindObjectsOfType<JsonSaveableEntity>())
            {
                stateDictionary[saveableEntity.GetUniqueIdentifier()] = saveableEntity.CaptureAsJToken();
            }

            stateDictionary[BUILD_INDEX_LABEL] = SceneManager.GetActiveScene().buildIndex;
        }

        private void RestoreFromToken(JObject state)
        {
            IDictionary<string, JToken> stateDictionary = state;

            foreach (JsonSaveableEntity saveableEntity in FindObjectsOfType<JsonSaveableEntity>())
            {
                string id = saveableEntity.GetUniqueIdentifier();

                if (stateDictionary.ContainsKey(id))
                {
                    saveableEntity.RestoreFromJToken(stateDictionary[id]);
                }
            }
        }

        private string GetPathFromSaveFile(string saveFile)
        {
            return Path.Combine(Application.persistentDataPath, saveFile + SAVE_FILE_EXTENSION);
        }
    }
}