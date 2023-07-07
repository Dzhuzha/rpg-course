using RPG.Stats;
using TMPro;
using UnityEngine;

public class LevelDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text _levelValue;
    [SerializeField] private BaseStats _baseStats;
    
    private void Start()
    {
        UpdateLevelValue(_baseStats.CurrentLevel.value);
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
        _baseStats.LevelChanged += UpdateLevelValue;
    }
    
    private void Unsubscribe()
    {
        _baseStats.LevelChanged -= UpdateLevelValue;
    }
    
    private void UpdateLevelValue(int newLevelValue)
    {
        _levelValue.text = newLevelValue.ToString("N0");
    }
}