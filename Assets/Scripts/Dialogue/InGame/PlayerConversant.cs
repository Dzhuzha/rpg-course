using System;
using System.Collections.Generic;
using System.Linq;
using RPG.Core;
using UnityEngine;

namespace RPG.Dialogue
{
    public class PlayerConversant : MonoBehaviour
    {
        public event Action DialogueUpdated;

        private NPCConversant _npcConversant;
        private Dialogue _currentDialogue;
        private DialogueNode _currentNode;

        public bool IsChoosing { get; private set; }
        public bool IsActive => _currentDialogue != null;

        public void BeginDialogue(NPCConversant newConversant, Dialogue dialogueToStart)
        {
            _npcConversant = newConversant;
            _currentDialogue = dialogueToStart;
            _currentNode = _currentDialogue.GetRootNode();
            TriggerEnterAction();
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
            return  _currentDialogue.GetAllChildren(_currentNode).Count() > 0; //FilterOnCondition()
        }

        public void ChooseNextNode()
        {
            int answerVariantsCount = FilterOnCondition(_currentDialogue.GetPlayerResponseChildren(_currentNode)).Count();

            if (answerVariantsCount > 0)
            {
                IsChoosing = true;
                TriggerExitAction();
                DialogueUpdated?.Invoke();
                return;
            }

            DialogueNode[] children = FilterOnCondition(_currentDialogue.GetNPCResponseChildren(_currentNode)).ToArray();
            int npcAnswerOption = UnityEngine.Random.Range(0, children.Length);
            TriggerExitAction();
            
            if (children.Length < 1) { ResetDialogue(); return;}
            
            _currentNode = children[npcAnswerOption];
            TriggerEnterAction();
            DialogueUpdated?.Invoke();
        }

        public IEnumerable<DialogueNode> FilterOnCondition(IEnumerable<DialogueNode> inputNode)
        {
            foreach (DialogueNode node in inputNode)
            {
                if (node.CheckCondition(GetEvaluators()))
                {
                    yield return node;
                }
            }
        }

        private IEnumerable<IPredicateEvaluator> GetEvaluators()
        {
            return GetComponents<IPredicateEvaluator>();
        }

        public IEnumerable<DialogueNode> GetAnswerChoices()
        {
            return FilterOnCondition(_currentDialogue.GetPlayerResponseChildren(_currentNode));
        }

        public void SelectAnswerNode(UnityEngine.UI.Button buttonToUnsubscribe, DialogueNode chosenNode)
        {
            _currentNode = chosenNode;
            TriggerEnterAction();
            IsChoosing = false;
            buttonToUnsubscribe.onClick.RemoveListener(() => SelectAnswerNode(buttonToUnsubscribe, chosenNode));
            ChooseNextNode(); // We could delete this line if we want to duplicate player chose in the text 
        }

        public void ResetDialogue()
        {
            TriggerExitAction();
            _npcConversant = null;
            _currentDialogue = null;
            _currentNode = null;
            IsChoosing = false;
            DialogueUpdated?.Invoke();
        }

        private void TriggerEnterAction()
        {
            if (_currentNode != null)
            {
                TriggerAction(_currentNode.OnEnterAction);
            }
        }

        private void TriggerExitAction()
        {
            if (_currentNode != null)
            {
                TriggerAction(_currentNode.OnExitAction);
            }
        }

        private void TriggerAction(string action)
        {
            if (action == String.Empty) return;

            foreach (DialogueTrigger actionTrigger in _npcConversant.GetComponents<DialogueTrigger>())
            {
                actionTrigger.Trigger(action);
            }
        }
    }
}