using UnityEngine;

namespace RPG.UI
{
    public class ShowHideUI : MonoBehaviour
    {
        [SerializeField] private KeyCode _toggleKey = KeyCode.Escape;
        [SerializeField] private GameObject _uiContainer;
        [SerializeField] private GameObject _hudContainer;

        private void Start()
        {
            _uiContainer.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(_toggleKey))
            {
                _uiContainer.SetActive(!_uiContainer.activeSelf);
                _hudContainer.SetActive(!_uiContainer.activeSelf);
            }
        }
    }
}