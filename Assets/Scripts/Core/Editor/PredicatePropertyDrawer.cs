using System.Collections.Generic;
using System.Linq;
using RPG.Inventory;
using UnityEditor;
using UnityEngine;
using RPG.Quests;

namespace RPG.Core.Editor
{
    [CustomPropertyDrawer(typeof(Condition.Predicate))]
    public class PredicatePropertyDrawer : PropertyDrawer
    {
        private Dictionary<string, Quest> _quests;
        private Dictionary<string, InventoryItem> _items;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty predicate = property.FindPropertyRelative("_predicate");
            SerializedProperty parameters = property.FindPropertyRelative("_parameters");
            SerializedProperty negate = property.FindPropertyRelative("_negate");
            float propertyHeight = EditorGUI.GetPropertyHeight(predicate);
            position.height = propertyHeight;
            EditorGUI.PropertyField(position, predicate);

            PredicateType selectedPredicate = (PredicateType) predicate.enumValueIndex;

            if (selectedPredicate == PredicateType.Select) return;
            while (parameters.arraySize < 2)
            {
                parameters.InsertArrayElementAtIndex(0);
            }

            SerializedProperty parameterZero = parameters.GetArrayElementAtIndex(0);
            SerializedProperty parameterOne = parameters.GetArrayElementAtIndex(1);

            if (selectedPredicate == PredicateType.HasQuest ||
                selectedPredicate == PredicateType.CompletedQuest ||
                selectedPredicate == PredicateType.CompletedObjective)
            {
                position.y += propertyHeight;
                DrawQuest(position, parameterZero);
                
                if (selectedPredicate == PredicateType.CompletedObjective)
                {
                    position.y += propertyHeight;
                    DrawObjective(position, parameterOne, parameterZero);
                }
            }

            position.y += propertyHeight;
            EditorGUI.PropertyField(position, negate);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty predicate = property.FindPropertyRelative("_predicate");
            float propertyHeight = EditorGUI.GetPropertyHeight(predicate);

            PredicateType selectedPredicate = (PredicateType) predicate.enumValueIndex;
            switch (selectedPredicate)
            {
                case PredicateType.Select:
                    return propertyHeight;
                case PredicateType.HasLevel:
                case PredicateType.CompletedQuest:
                case PredicateType.HasQuest:
                case PredicateType.HasItem:
                case PredicateType.HasItemEquipped:
                    return propertyHeight * 3.0f;
                case PredicateType.CompletedObjective:
                case PredicateType.HasItems:
                case PredicateType.MinimalTrait:
                    return propertyHeight * 4.0f;
            }

            return propertyHeight * 2.0f;
        }

        private void BuildQuestList()
        {
            if (_quests != null) return;
            _quests = new Dictionary<string, Quest>();

            foreach (Quest quest in Resources.LoadAll<Quest>(""))
            {
                _quests[quest.Name] = quest;
            }
        }

        private void DrawQuest(Rect position, SerializedProperty element)
        {
            BuildQuestList();
            var names = _quests.Keys.ToList();
            int index = names.IndexOf(element.stringValue);

            EditorGUI.BeginProperty(position, new GUIContent("Quest:"), element);
            int newIndex = EditorGUI.Popup(position, "Quest:", index, names.ToArray());
            if (newIndex != index)
            {
                element.stringValue = names[newIndex];
            }

            EditorGUI.EndProperty();
        }

        private void DrawObjective(Rect position, SerializedProperty element, SerializedProperty selectedQuest)
        {
            string questName = selectedQuest.stringValue;
            if (!_quests.ContainsKey(questName))
            {
                EditorGUI.HelpBox(position, "Please Select A Quest", MessageType.Error);
                return;
            }
            
            List<string> references = new List<string>();
            List<string> descriptions = new List<string>();

            foreach (Quest.Objective objective in _quests[questName].GetObjectives())
            {
                references.Add(objective.Reference);
                descriptions.Add(objective.Description);
            }

            int index = references.IndexOf(element.stringValue);
            EditorGUI.BeginProperty(position, new GUIContent("objective"), element);
            int newIndex = EditorGUI.Popup(position, "Objective", index, descriptions.ToArray());
           
            if (newIndex != index)
            {
                element.stringValue = references[newIndex];
            }
            
            EditorGUI.EndProperty();
        }
    }
}