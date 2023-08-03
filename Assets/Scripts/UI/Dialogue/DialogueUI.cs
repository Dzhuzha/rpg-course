using UnityEngine;
using RPG.Dialogue;
using TMPro;
using UnityEngine.UI;

namespace RPG.UI
{
    public class DialogueUI : MonoBehaviour
    {
        [SerializeField] private Button _continueButton;
        [SerializeField] private Button _closeButton;
        [SerializeField] private TMP_Text _text;
        private PlayerConversant _playerConversant;

        private void Awake()
        {
            _playerConversant = FindObjectOfType<PlayerConversant>();
        }

        private void OnEnable()
        {
            _closeButton.onClick.AddListener(Close);
            _continueButton.onClick.AddListener(Next);
        }

        private void OnDisable()
        {
            _closeButton.onClick.AddListener(Close);
            _continueButton.onClick.RemoveListener(Next);
        }

        private void Start()
        {
            UpdateUI();
        }

        private void Next()
        {
            _playerConversant.ChooseNextNode();
            UpdateUI();
        }

        private void UpdateUI()
        {
            _continueButton.gameObject.SetActive(_playerConversant.TryGetNext());
            _text.text = _playerConversant.GetText();
        }

        private void Close()
        {
            gameObject.SetActive(false);
        }
    }
}