using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RPG.Dialogue
{
    public class DialogueNode : ScriptableObject
    {
        [SerializeField] private bool _isPlayerSpeaking;
        [SerializeField] private string _speakerName;
        [SerializeField] private string _text;
        [SerializeField] private List<string> _children = new List<string>();
        [SerializeField] private Rect _rect;
        
        public string Text => _text;
        public string SpeakerName => _speakerName;
        public List<string> Children => _children;
        public Rect Rect => _rect;
        public bool IsPlayerSpeaking => _isPlayerSpeaking;

#if UNITY_EDITOR
        public void SetText(string textToSet)
        {
            if (textToSet != _text)
            {
                Undo.RecordObject(this, "Update Dialogue Text");
                _text = textToSet;
                EditorUtility.SetDirty(this);
            }
        }

        public void SetPosition(Vector2 newPosition)
        {
            Undo.RecordObject(this, "Move Dialogue Node");
            _rect.position = newPosition;
            EditorUtility.SetDirty(this);
        }

        public void SetChild(string childId)
        {
            Undo.RecordObject(this, "Add Dialogue Link");
            _children.Add(childId);
            EditorUtility.SetDirty(this);
        }

        public void RemoveChild(string childId)
        {
            Undo.RecordObject(this, "Remove Dialogue Link");
            _children.Remove(childId);
            EditorUtility.SetDirty(this);
        }

        public void SetSize(Vector2 size)
        {
            _rect.width = size.x;
            _rect.height = size.y;
            EditorUtility.SetDirty(this);
        }

        public void SetSpeaker(bool isPlayerSpeaker)
        {
            Undo.RecordObject(this, "Change Dialogue Speaker");
            _isPlayerSpeaking = isPlayerSpeaker;
            EditorUtility.SetDirty(this);
        }
        
        public void SetSpeakerName(string speakersName)
        {
            Undo.RecordObject(this, "Change Speaker Name");
            _speakerName = speakersName; 
            EditorUtility.SetDirty(this);
        }
#endif
    }
}