using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using Unity.Services.CloudSave;
using Unity.Services.Core;

namespace RPG.CloudServices
{
    public class CloudSaving : MonoBehaviour
    {
        [SerializeField] private UnityAuthenticator _authenticator;

        private void Awake()
        {
       //     _authenticator.AuthSucceed += 
        }

        //  AuthSuccessed
        public async void Start()
        {
            await UnityServices.InitializeAsync();
        }
        
        public async void SaveData(Dictionary<string, object> state)
        {
            var data = new Dictionary<string, object>() {{"data", state}};
            await CloudSaveService.Instance.Data.ForceSaveAsync(data);
        }
        
        public async void LoadData()
        {
            Dictionary<string, string> serverData = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> {"data"});
        
            if (serverData.ContainsKey("data"))
            {
               Debug.Log(serverData["data"]);
               BinaryFormatter formatter = new BinaryFormatter();
               using (MemoryStream stream = new MemoryStream())
               {
                 //  var dataLoaded = (Dictionary<string, object>)serverData["data"];
               }
            }
            else
            {
                Debug.LogWarning("COULD NOT LOAD THE DATA FROM CLOUD!");
            }
        }
        
        public async void DeleteKey()
        {
            await CloudSaveService.Instance.Data.ForceDeleteAsync("data");
        }
    }
}