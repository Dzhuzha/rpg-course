using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace RPG.Dialogue.Editor
{
    public class DialogueEditor : EditorWindow
    {
        [MenuItem("Window/Dialogue Editor")]
        public static void ShowEditorWindow()
        {
            GetWindow(typeof(DialogueEditor), false, "Dialogue Editor");
        }

        [OnOpenAssetAttribute(1)]
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
    }
}