using UnityEngine;

namespace RPG.UI.DamageText
{
    public class DamageTextSpawner : MonoBehaviour
    {
        [SerializeField] private DamageText damageTextPrefab;

        private DamageText _instance;

        public void Spawn(float damageAmount)
        {
            if (_instance != null)
            {
                Destroy(_instance.gameObject);
            }

            _instance = Instantiate(damageTextPrefab, transform);
            _instance.SetValue(damageAmount);
        }
    }
}