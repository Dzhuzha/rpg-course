using TMPro;
using UnityEngine;

namespace RPG.UI.DamageText
{
    public class DamageText : MonoBehaviour
    {
        [SerializeField] private TMP_Text _damage;
        
        public void SetValue(float damageAmount)
        {
            _damage.text = string.Concat(damageAmount.ToString("N0"));
        }
    }
}