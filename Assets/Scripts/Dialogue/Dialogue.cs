using UnityEngine;

namespace RPG.Dialogue
{
    [CreateAssetMenu(fileName = "Dialogue", menuName = "Dialogue/New Dialogue", order = 53)]
    public class Dialogue : ScriptableObject
    {
        [SerializeField, NonReorderable] private DialogueNode[] _nodes;
    }
}