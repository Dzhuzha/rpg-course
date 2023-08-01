using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace RPG.Dialogue.Editor
{
    public class DialogueEditor : EditorWindow
    {
        private Dialogue _dialogue;
        private GUIStyle _nodeStyle;
        private DialogueNode _draggingNode;
        private Vector2 _draggingOffset;

        [MenuItem("Window/Dialogue Editor")]
        public static void ShowEditorWindow()
        {
            GetWindow(typeof(DialogueEditor), false, "Dialogue Editor");
        }

        [OnOpenAsset(1)]
        public static bool OnOpenAsset(int instanceId, int line)
        {
            Dialogue dialogueAsset = EditorUtility.InstanceIDToObject(instanceId) as Dialogue;

            if (dialogueAsset)
            {
                ShowEditorWindow();
                return true;
            }

            return false;
        }

        private void OnEnable()
        {
            Selection.selectionChanged += AssignDialogue;

            _nodeStyle = new GUIStyle();
            _nodeStyle.normal.background = EditorGUIUtility.Load("node0") as Texture2D;
            _nodeStyle.padding = new RectOffset(20, 20, 15, 15);
            _nodeStyle.border = new RectOffset(12,12,12,12);
        }

        private void OnDisable()
        {
            Selection.selectionChanged -= AssignDialogue;
        }

        private void AssignDialogue()
        {
            if (Selection.activeObject is Dialogue)
            {
                _dialogue = Selection.activeObject as Dialogue;
                Repaint();
            }
            else
            {
                _dialogue = null;
            }
        }

        private void OnGUI()
        {
            if (_dialogue)
            {
                ProcessEvents();

                foreach (DialogueNode node in _dialogue.GetAllNodes())
                {
                    OnGUINode(node);
                }
            }
            else
            {
                EditorGUILayout.LabelField("No dialogue selected");
            }
        }

        private void ProcessEvents()
        {
            if (Event.current.type == EventType.MouseDown && _draggingNode == null)
            {
                _draggingNode = GetNodeAtPoint(Event.current.mousePosition);
             
                if (_draggingNode != null)
                {
                    _draggingOffset = _draggingNode.Position.position - Event.current.mousePosition;
                }
            }
            else if (Event.current.type == EventType.MouseDrag && _draggingNode != null)
            {
                Undo.RecordObject(_dialogue, "Move Dialogue Node");
                _draggingNode.SetPosition(Event.current.mousePosition + _draggingOffset);
                GUI.changed = true;
            }
            else if (Event.current.type == EventType.MouseUp && _draggingNode != null)
            {
                _draggingNode = null;
            }
        }
        
        private DialogueNode GetNodeAtPoint(Vector2 point)
        {
            DialogueNode chosenNode = null;
            
            foreach (var node in _dialogue.GetAllNodes())
            {
                if (node.Position.Contains(point))
                {
                    chosenNode = node;
                }
            }
            
            return chosenNode;
        }

        private void OnGUINode(DialogueNode node)
        {
            GUILayout.BeginArea(node.Position, _nodeStyle);
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.LabelField("Node:");
            string nodeNewId = EditorGUILayout.TextField(node.Id);
            string nodeNewText = EditorGUILayout.TextField(node.Text);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_dialogue, "Update Node Data");

                node.SetId(nodeNewId);
                node.SetText(nodeNewText);
            }

            GUILayout.EndArea();
        }
    }
}