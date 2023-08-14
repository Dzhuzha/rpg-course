using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace RPG.Saving
{
    public interface IJsonSaveable
    {
        JToken CaptureAsJToken();
        void RestoreFromJToken(JToken state);
    }

    public static class JsonStatics
    {
        public static JToken ToToken(this Vector3 vector)
        {
            JObject state = new JObject();
            IDictionary<string, JToken> stateDictionary = state;
            stateDictionary["x"] = vector.x;
            stateDictionary["y"] = vector.y;
            stateDictionary["z"] = vector.z;

            return state;
        }

        public static Vector3 ToVector3(this JToken state)
        {
            Vector3 vector = new Vector3();

            if (state is JObject jObject)
            {
                IDictionary<string, JToken> stateDictionary = jObject;

                if (stateDictionary.TryGetValue("x", out JToken x))
                {
                    vector.x = x.ToObject<float>();
                }

                if (stateDictionary.TryGetValue("y", out JToken y))
                {
                    vector.y = y.ToObject<float>();
                }

                if (stateDictionary.TryGetValue("z", out JToken z))
                {
                    vector.z = z.ToObject<float>();
                }
            }

            return vector;
        }
    }
}