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
    }
}