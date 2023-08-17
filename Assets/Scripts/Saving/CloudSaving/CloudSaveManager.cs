using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RPG.CloudServices;
using Unity.Services.CloudSave;
using UnityEngine;

namespace RPG.Saving
{
    public class CloudSaveManager : MonoBehaviour
    {
        [SerializeField] private UnityAuthenticator _authenticator;

        private string _playerId = String.Empty;
        public bool CloudServiceInitialized { get; private set; }

        private void Awake()
        {
            _authenticator.AuthSucceed += Init;
        }

        private void OnDestroy()
        {
            _authenticator.AuthSucceed -= Init;
        }

        private void Init(string playerId)
        {
            _playerId = playerId;
            CloudServiceInitialized = true;
        }

        public async Task<string> Load()
        {
           Dictionary<string, string> loadedData = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string>{_playerId});
           return loadedData[_playerId];
        }

        public async void Save(string dataToSave)
        {
            var data = new Dictionary<string, object> {{_playerId, dataToSave}};
            await CloudSaveService.Instance.Data.ForceSaveAsync(data);
        }
    }
}