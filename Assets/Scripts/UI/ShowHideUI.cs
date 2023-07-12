using UnityEngine;

namespace RPG.UI
{
    public class ShowHideUI : MonoBehaviour
    {
        [SerializeField] KeyCode _toggleKey = KeyCode.Escape;
        [SerializeField] GameObject _uiContainer;

        private void Start()
        {
            _uiContainer.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(_toggleKey))
            {
                _uiContainer.SetActive(!_uiContainer.activeSelf);
            }
        }
    }
}