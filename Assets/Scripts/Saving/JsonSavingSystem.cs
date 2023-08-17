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

        private JObject _state;
        private bool _isLoading;

        private const string SAVE_FILE_EXTENSION = ".json";
        private const string BUILD_INDEX_LABEL = "lastSceneBuildIndex";

        public IEnumerator LoadLastScene(string saveFile)
        {
            while (_cloudSaveManager.CloudServiceInitialized == false)
            {
                yield return null;
            }

            LoadFromCloud(saveFile);
            while (_isLoading)
            {
                yield return null;
            }

            IDictionary<string, JToken> stateDict = _state;
            int buildIndex = SceneManager.GetActiveScene().buildIndex;

            if (_state.ContainsKey(BUILD_INDEX_LABEL))
            {
                buildIndex = (int) _state[BUILD_INDEX_LABEL];
            }

            yield return SceneManager.LoadSceneAsync(buildIndex);
            RestoreFromToken();
        }

        public void Save(string saveFile)
        {
            CaptureAsToken();
            SaveFileAsJson(saveFile);
            SaveToCloud(saveFile);
        }

        public void Delete(string saveFile)
        {
            File.Delete(GetPathFromSaveFile(saveFile));
        }

        public void Load(string saveFile)
        {
            LoadFromCloud(saveFile);
            LoadJsonFromFile(saveFile);
            RestoreFromToken();
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

        private void SaveFileAsJson(string saveFile)
        {
            string path = GetPathFromSaveFile(saveFile);

            Debug.Log($"Saving to {path}");

            using (StreamWriter textWriter = File.CreateText(path))
            {
                using (JsonTextWriter writer = new JsonTextWriter(textWriter))
                {
                    writer.Formatting = Formatting.Indented;
                    _state.WriteTo(writer);
                }

                textWriter.Close();
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
            _isLoading = true;
            string path = GetPathFromSaveFile(saveFile);
            string json = await _cloudSaveManager.Load();

            using (StreamWriter textWriter = File.CreateText(path))
            {
                await textWriter.WriteAsync(json);
                textWriter.Close();
            }

            _state = LoadJsonFromFile(saveFile);
            _isLoading = false;
        }

        private JObject LoadJsonFromFile(string saveFile)
        {
            string path = GetPathFromSaveFile(saveFile);

            if (!File.Exists(path) || string.IsNullOrEmpty(File.ReadAllText(path)))
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

        private void CaptureAsToken()
        {
            IDictionary<string, JToken> stateDictionary = _state;

            foreach (JsonSaveableEntity saveableEntity in FindObjectsOfType<JsonSaveableEntity>())
            {
                stateDictionary[saveableEntity.GetUniqueIdentifier()] = saveableEntity.CaptureAsJToken();
            }

            stateDictionary[BUILD_INDEX_LABEL] = SceneManager.GetActiveScene().buildIndex;
        }

        private void RestoreFromToken()
        {
            IDictionary<string, JToken> stateDictionary = _state;

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