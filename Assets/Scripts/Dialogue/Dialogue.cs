using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace RPG.Dialogue
{
    [CreateAssetMenu(fileName = "Dialogue", menuName = "Dialogue/New Dialogue", order = 53)]
    public class Dialogue : ScriptableObject
    {
        [SerializeField, NonReorderable] private List<DialogueNode> _nodes = new List<DialogueNode>();

        private Dictionary<string, DialogueNode> _nodeLookup = new Dictionary<string, DialogueNode>();
        
        public IEnumerable<DialogueNode> GetAllNodes()
        {
            return _nodes;
        }

        public DialogueNode GetRootNode()
        {
            return _nodes[0];
        }

        private void OnValidate()
        {
            _nodeLookup.Clear();

            foreach (DialogueNode node in _nodes)
            {
                _nodeLookup[node.Id] = node;
            }
        }


        private void Awake()
        {
#if UNITY_EDITOR
            if (_nodes.Count < 1)
            {
                _nodes.Add(new DialogueNode());
            }
#endif
            OnValidate();
        }

        public IEnumerable<DialogueNode> GetAllChildren(DialogueNode parentNode)
        {
            foreach (var childId in parentNode.Children)
            {
                if (_nodeLookup.ContainsKey(childId))
                {
                    yield return _nodeLookup[childId];
                }
            }
            
            // foreach (var node in parentNode.Children)
            // {
            //     yield return _nodes.First(n => n.Id == node);
            // }
        }
    }
}