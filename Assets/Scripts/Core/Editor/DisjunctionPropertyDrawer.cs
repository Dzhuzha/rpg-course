using System.Collections;
using UnityEditor;
using UnityEngine;

namespace RPG.Core.Editor
{
    [CustomPropertyDrawer(typeof(Condition.Disjunction))]
    public class DisjunctionPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUIStyle style = new GUIStyle(EditorStyles.label);
            style.alignment = TextAnchor.MiddleCenter;
            SerializedProperty or = property.FindPropertyRelative("_or");
            float propertyHeight = EditorGUIUtility.singleLineHeight;
            Rect upPosition = position;
            upPosition.width -= EditorGUIUtility.labelWidth;
            upPosition.x = position.xMax - upPosition.width;
            upPosition.width /= 3.0f;
            upPosition.height = propertyHeight;
            Rect downPosition = upPosition;
            downPosition.x += upPosition.width;
            Rect deletePosition = upPosition;
            deletePosition.x = position.xMax - deletePosition.width;
            int itemToRemove = -1;
            int itemToMoveUp = -1;
            int itemMoveDown = -1;

            var enumerator = or.GetEnumerator();

            int index = 0;

            while (enumerator.MoveNext())
            {
                if (index > 0)
                {
                    if (GUI.Button(downPosition, "v")) itemMoveDown = index - 1;
                    EditorGUI.DropShadowLabel(position, "--------OR--------", style);
                    position.y += propertyHeight;
                }

                SerializedProperty prop = enumerator.Current as SerializedProperty;
                position.height = EditorGUI.GetPropertyHeight(prop);
                EditorGUI.PropertyField(position, prop);
                position.y += position.height;
                position.height = propertyHeight;

                upPosition.y = deletePosition.y = downPosition.y = position.y;
                if (GUI.Button(deletePosition, "Remove")) itemToRemove = index;
                if (index > 0 && GUI.Button(upPosition, "^")) itemToMoveUp = index;
                position.y += propertyHeight;

                index++;
            }

            if (itemToRemove >= 0)
            {
                or.DeleteArrayElementAtIndex(itemToRemove);
            }

            if (itemToMoveUp >= 0)
            {
                or.MoveArrayElement(itemToMoveUp, itemToMoveUp - 1);
            }

            if (itemMoveDown >= 0)
            {
                or.MoveArrayElement(itemMoveDown, itemMoveDown + 1);
            }

            if (GUI.Button(position, "Add Predicate"))
            {
                or.InsertArrayElementAtIndex(index);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float result = 0;
            float propertyHeight = EditorGUIUtility.singleLineHeight;
            IEnumerator enumerator = property.FindPropertyRelative("_or").GetEnumerator();
            bool multiply = false;
            
            while (enumerator.MoveNext())
            {
                SerializedProperty prop = enumerator.Current as SerializedProperty;
                result += EditorGUI.GetPropertyHeight(prop) + propertyHeight;
                if (multiply) result += propertyHeight;
                multiply = true;
            }

            return result + propertyHeight * 1.5f;
        }
    }
}