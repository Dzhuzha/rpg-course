using UnityEngine;
using UnityEngine.Events;

namespace RPG.Dialogue
{
    public class DialogueTrigger : MonoBehaviour
    {
        [SerializeField] private string _action;
        [SerializeField] private UnityEvent _triggered;

        public void Trigger(string actionToTrigger)
        {
            if (actionToTrigger != _action) return;

            _triggered.Invoke();
        }
    }
}