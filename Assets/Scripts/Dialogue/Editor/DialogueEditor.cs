using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace RPG.Dialogue.Editor
{
    public class DialogueEditor : EditorWindow
    {
        [NonSerialized] private GUIStyle _nodeStyle;
        [NonSerialized] private DialogueNode _draggingNode;
        [NonSerialized] private Vector2 _draggingOffset;
        [NonSerialized] private DialogueNode _creatingNode;
        [NonSerialized] private DialogueNode _deletingNode;

        private Dialogue _dialogue;

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
            _nodeStyle.border = new RectOffset(12, 12, 12, 12);
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
                    DrawConnections(node);
                }

                foreach (DialogueNode node in _dialogue.GetAllNodes())
                {
                    DrawNode(node);
                }

                if (_creatingNode != null)
                {
                    Undo.RecordObject(_dialogue, "Added Dialogue Node");
                    _dialogue.CreateNewNode(_creatingNode);
                    _creatingNode = null;
                }

                if (_deletingNode != null)
                {
                    Undo.RecordObject(_dialogue, "Deleted Dialogue Node");
                    _dialogue.DeleteNode(_deletingNode);
                    _deletingNode = null;
                }
            }
            else
            {
                EditorGUILayout.LabelField("No dialogue selected");
            }
        }

        private void DrawConnections(DialogueNode node)
        {
            Vector2 startPosition = new Vector2(node.Rect.xMax, node.Rect.center.y);

            foreach (DialogueNode childNode in _dialogue.GetAllChildren(node))
            {
                Vector2 endPosition = new Vector2(childNode.Rect.xMin, childNode.Rect.center.y);
                Vector2 controlPointOffset = endPosition - startPosition;
                controlPointOffset.y = 0;
                controlPointOffset.x *= 0.8f;

                Handles.DrawBezier(startPosition, endPosition, startPosition + controlPointOffset,
                    endPosition - controlPointOffset, Color.white, null, 4f);
            }
        }

        private void ProcessEvents()
        {
            if (Event.current.type == EventType.MouseDown && _draggingNode == null)
            {
                _draggingNode = GetNodeAtPoint(Event.current.mousePosition);

                if (_draggingNode != null)
                {
                    _draggingOffset = _draggingNode.Rect.position - Event.current.mousePosition;
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
                if (node.Rect.Contains(point))
                {
                    chosenNode = node;
                }
            }

            return chosenNode;
        }

        private void DrawNode(DialogueNode node)
        {
            GUILayout.BeginArea(node.Rect, _nodeStyle);
            EditorGUI.BeginChangeCheck();

            string nodeNewText = EditorGUILayout.TextField(node.Text);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_dialogue, "Update Node Data");

                node.SetText(nodeNewText);
            }

            DrawNodeButtons(node);
            GUILayout.EndArea();
        }

        private void DrawNodeButtons(DialogueNode node)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("x"))
            {
                _deletingNode = node;
            }
            
            if (GUILayout.Button("+"))
            {
                _creatingNode = node;
            }
            GUILayout.EndHorizontal();
        }
    }
}