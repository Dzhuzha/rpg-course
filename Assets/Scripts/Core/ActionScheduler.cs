using UnityEngine;

namespace RPG.Core
{
    public class ActionScheduler : MonoBehaviour
    {
        private IAction _currentAction;
        public bool IsDisabled; 
        
        public void StartAction(IAction action)
        {
            if (_currentAction == action || IsDisabled) return;

            if (_currentAction != null)
            {
                _currentAction.CancelAction();
            }

            _currentAction = action;
        }
        
        public void CancelCurrentAction()
        {
            if (_currentAction != null)
            {
                _currentAction.CancelAction();
            }
            
            StartAction(null);
        }
    }
}