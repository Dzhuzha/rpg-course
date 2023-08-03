using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace RPG.Dialogue
{
    public class PlayerConversant : MonoBehaviour
    {
        [SerializeField] private Dialogue _currentDialogue;
        private DialogueNode _currentNode;

        private void Awake()
        {
            _currentNode = _currentDialogue.GetRootNode();
        }

        public string GetText()
        {
            if (_currentNode == null)
            {
                return String.Empty;
            }


            return _currentNode.Text;
        }

        public bool TryGetNext()
        {
            return _currentNode.Children.Count > 0;
        }

        public void ChooseNextNode()
        {
            DialogueNode[] children = _currentDialogue.GetAllChildren(_currentNode).ToArray();
            int npcAnswerOption = 0;
            
            if (!_currentNode.IsPlayerSpeaking)
            {
                npcAnswerOption = Random.Range(0, children.Length - 1);
            }

            _currentNode = children[npcAnswerOption];
        }
    }
}