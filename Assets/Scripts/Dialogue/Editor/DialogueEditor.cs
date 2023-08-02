using System;
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
        [NonSerialized] private DialogueNode _linkingParentNode;
        [NonSerialized] private Vector2 _draggingCanvasOffset;
        [NonSerialized] private bool _draggingCanvas;

        private Dialogue _dialogue;
        private Vector2 _scrollPosition;

        private const float CANVAS_SIZE = 4000f;
        private const float BACKGROUND_SIZE = 50f;

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
            Dialogue newDialogue = Selection.activeObject as Dialogue;

            if (newDialogue != null)
            {
                _dialogue = newDialogue;
                Repaint();
            }
        }

        private void OnGUI()
        {
            if (_dialogue)
            {
                ProcessEvents();

                _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
                Rect canvas = GUILayoutUtility.GetRect(CANVAS_SIZE, CANVAS_SIZE);
                Texture2D backgroundTexture = Resources.Load("background") as Texture2D;
                Rect textCoords = new Rect(0, 0, CANVAS_SIZE / BACKGROUND_SIZE, CANVAS_SIZE / BACKGROUND_SIZE);
                GUI.DrawTextureWithTexCoords(canvas, backgroundTexture, textCoords);

                foreach (DialogueNode node in _dialogue.GetAllNodes())
                {
                    DrawConnections(node);
                }

                foreach (DialogueNode node in _dialogue.GetAllNodes())
                {
                    DrawNode(node);
                }

                EditorGUILayout.EndScrollView();

                if (_creatingNode != null)
                {
                    _dialogue.CreateNewNode(_creatingNode);
                    _creatingNode = null;
                }

                if (_deletingNode != null)
                {
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
                _draggingNode = GetNodeAtPoint(Event.current.mousePosition + _scrollPosition);

                if (_draggingNode != null)
                {
                    _draggingOffset = _draggingNode.Rect.position - Event.current.mousePosition;
                    Selection.activeObject = _draggingNode;
                }
                else
                {
                    _draggingCanvas = true;
                    _draggingCanvasOffset = Event.current.mousePosition + _scrollPosition;
                    Selection.activeObject = _dialogue;
                }
            }
            else if (Event.current.type == EventType.MouseDrag && _draggingNode != null)
            {
                _draggingNode.SetPosition(Event.current.mousePosition + _draggingOffset);
                GUI.changed = true;
            }
            else if (Event.current.type == EventType.MouseDrag && _draggingCanvas)
            {
                _scrollPosition = _draggingCanvasOffset - Event.current.mousePosition;
                GUI.changed = true;
            }
            else if (Event.current.type == EventType.MouseUp && _draggingNode != null)
            {
                _draggingNode = null;
            }
            else if (Event.current.type == EventType.MouseUp && _draggingCanvas)
            {
                _draggingCanvas = false;
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
            
            node.SetText(EditorGUILayout.TextField(node.Text));

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

            DrawLinkButtons(node);

            if (GUILayout.Button("+"))
            {
                _creatingNode = node;
            }

            GUILayout.EndHorizontal();
        }

        private void DrawLinkButtons(DialogueNode node)
        {
            if (_linkingParentNode == null)
            {
                if (GUILayout.Button("link"))
                {
                    _linkingParentNode = node;
                }
            }
            else if (ReferenceEquals(_linkingParentNode, node))
            {
                if (GUILayout.Button("cancel"))
                {
                    _linkingParentNode = null;
                }
            }
            else if (_linkingParentNode.Children.Contains(node.name))
            {
                if (GUILayout.Button("unlink"))
                {
                    _linkingParentNode.RemoveChild(node.name);
                    _linkingParentNode = null;
                }
            }
            else
            {
                if (GUILayout.Button("child"))
                {
                    Undo.RecordObject(_dialogue, "Added Dialogue Link");
                    _linkingParentNode.SetChild(node.name);
                    _linkingParentNode = null;
                }
            }
        }
    }
}