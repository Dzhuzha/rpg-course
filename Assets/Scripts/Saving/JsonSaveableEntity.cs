using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

namespace RPG.Saving
{
    [ExecuteAlways]
    public class JsonSaveableEntity : MonoBehaviour
    {
        [SerializeField] private string _uniqueIdentifier = String.Empty;
        
        private static Dictionary<string, JsonSaveableEntity> _globalLookUp = new Dictionary<string, JsonSaveableEntity>();

        public string GetUniqueIdentifier()
        {
            return _uniqueIdentifier;
        }

        public JToken CaptureAsJToken()
        {
            JObject state = new JObject();
            IDictionary<string, JToken> stateDictionary = state;

            foreach (IJsonSaveable jsonSaveable in GetComponents<IJsonSaveable>())
            {
                JToken token = jsonSaveable.CaptureAsJToken();
                string component = jsonSaveable.GetType().ToString();

                Debug.Log($"{name} Capture {component} = {token}");
                stateDictionary[jsonSaveable.GetType().ToString()] = token;
            }

            return state;
        }

        public void RestoreFromJToken(JToken stateToRestore)
        {
            JObject state = stateToRestore.ToObject<JObject>();
            IDictionary<string, JToken> stateDictionary = state;

            foreach (IJsonSaveable jsonSaveable in GetComponents<IJsonSaveable>())
            {
                string component = jsonSaveable.GetType().ToString();

                if (stateDictionary.ContainsKey(component))
                {
                    Debug.Log($"{name} Restore {component} = {stateDictionary[component]}");
                    jsonSaveable.RestoreFromJToken(stateDictionary[component]);
                }
            }
        }

        private bool IsUnique(string candidate)
        {
            if (!_globalLookUp.ContainsKey(candidate)) return true;
            if (_globalLookUp[candidate] == this) return true;
            
            if (_globalLookUp[candidate] == null)
            {
                _globalLookUp.Remove(candidate);
                return true;
            }

            if (_globalLookUp[candidate].GetUniqueIdentifier() != candidate)
            {
                _globalLookUp.Remove(candidate);
                return true;
            }

            return false;
        }
        
#if UNITY_EDITOR
        private void Update()
        {
            if (Application.IsPlaying(gameObject)) return;
            if (string.IsNullOrEmpty(gameObject.scene.path)) return;
    
            SerializedObject  serializedObject = new SerializedObject(this);
            SerializedProperty property = serializedObject.FindProperty("_uniqueIdentifier");

            if (string.IsNullOrEmpty(property.stringValue) || !IsUnique(property.stringValue))
            {
                property.stringValue = Guid.NewGuid().ToString();
                serializedObject.ApplyModifiedProperties();
            }

            _globalLookUp[property.stringValue] = this;
        }
#endif
    }
}