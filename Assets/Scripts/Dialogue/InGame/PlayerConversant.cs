using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPG.Dialogue
{
    public class PlayerConversant : MonoBehaviour
    {
        public event Action DialogueUpdated;
        
        [SerializeField] private Dialogue _testDialogue;

        private Dialogue _currentDialogue;
        private DialogueNode _currentNode;

        public bool IsChoosing { get; private set; }
        public bool IsActive => _currentDialogue != null;

        public void BeginDialogue(Dialogue dialogueToStart)
        {
            _currentDialogue = dialogueToStart;
            _currentNode = _currentDialogue.GetRootNode();
            DialogueUpdated?.Invoke();
        }

        public string GetText()
        {
            return _currentNode == null ? String.Empty : _currentNode.Text;
        }

        public string GetSpeakersName()
        {
            return _currentNode == null ? String.Empty : _currentNode.SpeakerName;
        }

        public bool TryGetNext()
        {
            return _currentNode.Children.Count > 0;
        }

        public void ChooseNextNode()
        {
            int answerVariantsCount = _currentDialogue.GetPlayerResponseChildren(_currentNode).Count();

            if (answerVariantsCount > 0)
            {
                IsChoosing = true;
                DialogueUpdated?.Invoke();
                return;
            }

            DialogueNode[] children = _currentDialogue.GetNPCResponseChildren(_currentNode).ToArray();
            int npcAnswerOption = UnityEngine.Random.Range(0, children.Length);
            _currentNode = children[npcAnswerOption];
            DialogueUpdated?.Invoke();
        }

        public IEnumerable<DialogueNode> GetAnswerChoices()
        {
            return _currentDialogue.GetPlayerResponseChildren(_currentNode);
        }

        public void SelectAnswerNode(UnityEngine.UI.Button buttonToUnsubscribe, DialogueNode chosenNode)
        {
            _currentNode = chosenNode;
            IsChoosing = false;
            buttonToUnsubscribe.onClick.RemoveListener(() => SelectAnswerNode(buttonToUnsubscribe, chosenNode));
            ChooseNextNode(); // We could delete this line if we want to duplicate player chose in the text 
        }

        public void ResetDialogue()
        {
            _currentDialogue = null;
            _currentNode = null;
            IsChoosing = false;
            DialogueUpdated?.Invoke();
        }
    }
}