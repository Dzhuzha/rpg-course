using RPG.Stats;
using TMPro;
using UnityEngine;

public class ExperienceDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text _experienceValue;
    [SerializeField] private Experience _experience;
    
    public void Start()
    {
        UpdateExperienceValue(_experience.ExperiencePoints);
    }

    private void OnEnable()
    {
        Subscribe();
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    private void Subscribe()
    {
        _experience.ExperienceChanged += UpdateExperienceValue;
    }
        
    private void Unsubscribe()
    {
        _experience.ExperienceChanged -= UpdateExperienceValue;
    }

    private void UpdateExperienceValue(float newExperienceValue)
    {
        _experienceValue.text = newExperienceValue.ToString("N0");
    }
}