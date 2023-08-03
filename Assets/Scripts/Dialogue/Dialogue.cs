using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RPG.Dialogue
{
    [CreateAssetMenu(fileName = "Dialogue", menuName = "Dialogue/New Dialogue", order = 53)]
    public class Dialogue : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField, NonReorderable] private List<DialogueNode> _nodes = new List<DialogueNode>();

        private const float NEW_NODE_OFFSET_X = 200f;
        private const float DEAFAULT_NODE_SIZE_X = 150f;
        private const float DEAFAULT_NODE_SIZE_Y = 100f;
        private Dictionary<string, DialogueNode> _nodeLookup = new Dictionary<string, DialogueNode>();

        private void Awake()
        {
            OnValidate();
        }

        private void OnValidate()
        {
            _nodeLookup.Clear();
            // if (_nodes == null) return;
            foreach (DialogueNode node in GetAllNodes())
            {
                _nodeLookup[node.name] = node;
            }
        }

        public IEnumerable<DialogueNode> GetAllNodes()
        {
            return _nodes;
        }

        public DialogueNode GetRootNode()
        {
            return _nodes[0];
        }

        public IEnumerable<DialogueNode> GetAllChildren(DialogueNode parentNode)
        {
            //if (parentNode.Children == null) return;
            foreach (var childId in parentNode.Children) 
            {
                if (_nodeLookup.ContainsKey(childId))
                {
                    yield return _nodeLookup[childId];
                }
            }
        }

#if UNITY_EDITOR
        public void CreateNewNode(DialogueNode parent)
        {
            DialogueNode newNode = MakeNode(parent);
            Undo.RegisterCreatedObjectUndo(newNode, "Created New Node");
            
            if (AssetDatabase.GetAssetPath(this) != String.Empty)
            {
                Undo.RecordObject(this, "Added Dialogue Node");
            }

            AddNode(newNode);
        }

        public void DeleteNode(DialogueNode nodeToDelete)
        {
            Undo.RecordObject(this, "Deleted Dialogue Node");
            _nodes.Remove(nodeToDelete);
            OnValidate();
            CleanDanglingChildren(nodeToDelete);
            Undo.DestroyObjectImmediate(nodeToDelete);
        }

        private DialogueNode MakeNode(DialogueNode parent)
        {
            Vector2 appearancePosition = Vector2.zero;
            DialogueNode newNode = CreateInstance<DialogueNode>();
            newNode.name = Guid.NewGuid().ToString();

            if (parent != null)
            {
                parent.SetChild(newNode.name);
                appearancePosition.x = parent.Rect.x + NEW_NODE_OFFSET_X;
                appearancePosition.y = parent.Rect.y;
                newNode.SetSpeaker(!parent.IsPlayerSpeaking);
            }
            
            newNode.SetPosition(appearancePosition);
            newNode.SetSize(new Vector2(DEAFAULT_NODE_SIZE_X, DEAFAULT_NODE_SIZE_Y));

            return newNode;
        }

        private void AddNode(DialogueNode newNode)
        {
            _nodes.Add(newNode);
            OnValidate();
        }

        private void CleanDanglingChildren(DialogueNode nodeToDelete)
        {
            foreach (DialogueNode node in GetAllNodes())
            {
                node.RemoveChild(nodeToDelete.name);
            }
        }
#endif

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
#if UNITY_EDITOR
            if (_nodes.Count == 0)
            {
               // CreateNewNode(null);
              DialogueNode newNode = MakeNode(null);
              AddNode(newNode);
            }

            if (AssetDatabase.GetAssetPath(this) != String.Empty)
            {
                foreach (DialogueNode node in GetAllNodes())
                {
                    if (AssetDatabase.GetAssetPath(node) == String.Empty)
                    {
                        AssetDatabase.AddObjectToAsset(node, this);
                    }
                }
            }
#endif
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
        }
    }
}