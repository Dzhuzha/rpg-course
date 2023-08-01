using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Dialogue
{
    [Serializable]
    public class DialogueNode
    {
        [SerializeField] private string _id;
        [SerializeField] private string _text;
        [SerializeField] private List<string> _children = new List<string>();
        [SerializeField] private Rect _rect;
        
        public string Id => _id;
        public string Text => _text;
        public List<string> Children => _children;
        public Rect Rect => _rect;

        public void SetText(string textToSet)
        {
            _text = textToSet;
        }
        
        public void SetId(string idToSet)
        {
            _id = idToSet;
        }

        public void SetPosition(Vector2 newPosition)
        {
            _rect.position = newPosition;
        }

        public void SetChild(string childId)
        {
            _children.Add(childId);
        }

        public void SetSize(Vector2 size)
        {
            _rect.width = size.x;
            _rect.height = size.y;
        }
    }
}