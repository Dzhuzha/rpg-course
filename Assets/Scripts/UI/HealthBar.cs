using RPG.Atributes;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private RectTransform _foreground;
    [SerializeField] private Health _health;
    [SerializeField] private Canvas _rootCanvas;
    
    private void Awake()
    {
        _foreground.localScale = new Vector3(1f, 1f, 1f);
    }

    private void OnEnable()
    {
       _health.HealthChanged += SetSize;
    }

    private void OnDisable()
    {
        _health.HealthChanged -= SetSize;
    }

    private void SetSize(float newHealth, float fullHealth)
    {
        if (newHealth >= fullHealth || newHealth <= 0f)
        {
            _rootCanvas.enabled = false;
        }
        else
        {
            _rootCanvas.enabled = true;
            var fraction = newHealth / fullHealth;
            _foreground.localScale = new Vector3(fraction, 1f, 1f);
        }
    }
}