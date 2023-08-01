using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Dialogue
{
    [CreateAssetMenu(fileName = "Dialogue", menuName = "Dialogue/New Dialogue", order = 53)]
    public class Dialogue : ScriptableObject
    {
        [SerializeField, NonReorderable] private List<DialogueNode> _nodes = new List<DialogueNode>();

        public IEnumerable<DialogueNode> GetAllNodes()
        {
            return _nodes;
        }

        public DialogueNode GetRootNode()
        {
            return _nodes[0];
        }

#if UNITY_EDITOR
        private void Awake()
        {
            if (_nodes.Count < 1)
            {
                _nodes.Add(new DialogueNode());
            }
        }
    }
#endif
}