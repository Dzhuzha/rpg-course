using System;
using UnityEngine;

namespace RPG.Dialogue
{
    public class PlayerConversant : MonoBehaviour
    {
        [SerializeField] private Dialogue _currentDialogue;

        public string GetText()
        {
            return _currentDialogue == null ? String.Empty : _currentDialogue.GetRootNode().Text;
        }
    }
}