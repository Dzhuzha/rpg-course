using TMPro;
using UnityEngine;

namespace RPG.Quests
{
    public class ObjectiveWidget : MonoBehaviour
    {
        [SerializeField] private GameObject _completeMark;
        [SerializeField] private TMP_Text _objectiveText;

        public void SetCompleteMark(bool completeStatus)
        {
            _completeMark.SetActive(completeStatus);

            if (completeStatus)
            {
                _objectiveText.fontStyle = FontStyles.Strikethrough;
            }
        }
        
        public void SetObjectiveText(string text)
        {
            _objectiveText.text = text;
        }
    }
}