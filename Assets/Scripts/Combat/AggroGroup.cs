using UnityEngine;

namespace RPG.Combat
{
    public class AggroGroup : MonoBehaviour
    {
        [SerializeField] private Fighter[] _fighters;
        [SerializeField] private bool _activateOnStart;
        
        private void Start()
        {
           Activate(_activateOnStart);
        }

        public void Activate(bool shouldActivate)
        {
            foreach (Fighter fighter in _fighters)
            {
                fighter.enabled = shouldActivate;
                fighter.GetComponent<CombatTarget>().enabled = shouldActivate;
            }
        }
    }
}