using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using RPG.Saving;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG.Inventory
{
    /// <summary>
    /// To be on anything that can drop items.
    /// Tracks the drops for saving and restoring.
    /// </summary>
    public class ItemDropper : MonoBehaviour, IJsonSaveable
    {
        private List<Pickup> _droppedItems = new List<Pickup>();
        private List<OtherSceneDropRecord> _otherSceneDrops = new List<OtherSceneDropRecord>();

        public void DropItem(InventoryItem item, int count)
        {
            SpawnPickup(item, count, GetDropLocation());
        }

        protected virtual Vector3 GetDropLocation()
        {
            return transform.position;
        }

        private void SpawnPickup(InventoryItem item, int count, Vector3 spawnLocation)
        {
            var pickup = item.SpawnPickup(spawnLocation, count);
            _droppedItems.Add(pickup);
        }

        private void RemoveDestroyedDrops()
        {
            _droppedItems.RemoveAll(pickup => pickup == null);
        }

        private class OtherSceneDropRecord
        {
            public string Id;
            public int Number;
            public Vector3 Location;
            public int SceneIndex;
        }

        private List<OtherSceneDropRecord> MergeDroppedItemsWithOtherSceneDrops()
        {
            List<OtherSceneDropRecord> result = new List<OtherSceneDropRecord>();
            result.AddRange(_otherSceneDrops);

            foreach (Pickup item in _droppedItems)
            {
                OtherSceneDropRecord drop = new OtherSceneDropRecord();
                drop.Id = item.Item.ItemID;
                drop.Number = item.Count;
                drop.Location = item.transform.position;
                drop.SceneIndex = SceneManager.GetActiveScene().buildIndex;
                result.Add(drop);
            }

            return result;
        }

        private void ClearExistingDrops()
        {
            foreach (Pickup oldDrop in _droppedItems)
            {
                if (oldDrop != null) 
                {
                    Destroy(oldDrop.gameObject);
                }
            }
            
            _otherSceneDrops.Clear();
        }

        public JToken CaptureAsJToken()
        {
            RemoveDestroyedDrops();
            var drops = MergeDroppedItemsWithOtherSceneDrops();
            JArray state = new JArray();
            IList<JToken> stateList = state;

            foreach (OtherSceneDropRecord otherSceneDrop in drops)
            {
                JObject dropState = new JObject();
                IDictionary<string, JToken> dropStateDictionary = dropState;
                dropStateDictionary["id"] = JToken.FromObject(otherSceneDrop.Id);
                dropStateDictionary["number"] = otherSceneDrop.Number;
                dropStateDictionary["location"] = otherSceneDrop.Location.ToToken();
                dropStateDictionary["scene"] = otherSceneDrop.SceneIndex;
                stateList.Add(dropState);
            }

            return state;
        }

        public void RestoreFromJToken(JToken state)
        {
            if (state is JArray stateArray)
            {
                int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
                IList<JToken> stateList = stateArray;
                ClearExistingDrops();

                foreach (JToken entry in stateList)
                {
                    if (entry is JObject dropState)
                    {
                        IDictionary<string, JToken> dropStateDictionary = dropState;
                        int sceneIndex = dropStateDictionary["scene"].ToObject<int>();
                        InventoryItem item = InventoryItem.GetFromID(dropStateDictionary["id"].ToObject<string>());
                        int number = dropStateDictionary["number"].ToObject<int>();
                        Vector3 location = dropStateDictionary["location"].ToVector3();

                        if (sceneIndex == currentSceneIndex)
                        {
                            SpawnPickup(item, number, location);
                        }
                        else
                        {
                            var otherDrop = new OtherSceneDropRecord();
                            otherDrop.Id = item.ItemID;
                            otherDrop.Number = number;
                            otherDrop.Location = location;
                            otherDrop.SceneIndex = sceneIndex;
                            _otherSceneDrops.Add(otherDrop);
                        }
                    }
                }
            }
        }
    }
}