using System;
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
        [SerializeField] private CloudSaveManager _cloudSaveManager;

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
            SaveFileAsJson(saveFile, state);
            SaveToCloud(saveFile);
        }

        public void Delete(string saveFile)
        {
            File.Delete(GetPathFromSaveFile(saveFile));
        }

        public void Load(string saveFile)
        {
            LoadFromCloud(saveFile);
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

        private void SaveFileAsJson(string saveFile, JObject state)
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

        private void SaveToCloud(string saveFile)
        {
            string path = GetPathFromSaveFile(saveFile);
            string json = String.Empty;

            if (!File.Exists(path))
            {
                return;
            }

            json = File.ReadAllText(path);
            _cloudSaveManager.Save(json);
        }

        private async void LoadFromCloud(string saveFile)
        {
            string path = GetPathFromSaveFile(saveFile);
            string json = await _cloudSaveManager.Load();

            if (!File.Exists(path))
            {
                File.Create(path);
                return;
            }

            await File.WriteAllTextAsync(path, json);
            RestoreFromToken(LoadJsonFromFile(saveFile));
            ;
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