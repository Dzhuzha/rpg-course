using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RPG.Dialogue
{
    public class DialogueNode : ScriptableObject
    {
        [SerializeField] private string _text;
        [SerializeField] private List<string> _children = new List<string>();
        [SerializeField] private Rect _rect;
        
        public string Text => _text;
        public List<string> Children => _children;
        public Rect Rect => _rect;

#if UNITY_EDITOR
        public void SetText(string textToSet)
        {
            if (textToSet != _text)
            {
                Undo.RecordObject(this, "Update Dialogue Text");
                _text = textToSet;
            }
        }

        public void SetPosition(Vector2 newPosition)
        {
            Undo.RecordObject(this, "Move Dialogue Node");
            _rect.position = newPosition;
        }

        public void SetChild(string childId)
        {
            Undo.RecordObject(this, "Add Dialogue Link");
            _children.Add(childId);
        }

        public void RemoveChild(string childId)
        {
            Undo.RecordObject(this, "Remove Dialogue Link");
            _children.Remove(childId);
        }

        public void SetSize(Vector2 size)
        {
            _rect.width = size.x;
            _rect.height = size.y;
        }
#endif
    }
}