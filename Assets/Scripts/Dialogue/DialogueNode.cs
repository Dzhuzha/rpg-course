using System;
using UnityEngine;

namespace RPG.Dialogue
{
    [Serializable]
    public class DialogueNode
    {
        [SerializeField] private string _id;
        [SerializeField] private string _text;
        [SerializeField] private string[] _children;
        [SerializeField] private Rect _position;
        
        public string Id => _id;
        public string Text => _text;
        public string[] Children => _children;
        public Rect Position => _position;

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
            _position.position = newPosition;
        }
    }
}