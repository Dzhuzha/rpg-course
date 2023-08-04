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
        [SerializeField] private Button _choiceButton;
        [SerializeField] private Transform _npcTextRoot;
        [SerializeField] private Transform _choiceRoot;
        [SerializeField] private TMP_Text _speakersName;
        [SerializeField] private TMP_Text _text;

        private PlayerConversant _playerConversant;

        private void Awake()
        {
            _playerConversant = FindObjectOfType<PlayerConversant>();
            Subscribe();
        }

        private void Start()
        {
            UpdateUI();
        }

        private void OnDestroy()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            _closeButton.onClick.AddListener(() => _playerConversant.ResetDialogue());
            _continueButton.onClick.AddListener(() => _playerConversant.ChooseNextNode());
            _playerConversant.DialogueUpdated += UpdateUI;
        }

        private void Unsubscribe()
        {
            _closeButton.onClick.RemoveListener(() => _playerConversant.ResetDialogue());
            _continueButton.onClick.RemoveListener(() => _playerConversant.ChooseNextNode());
            _playerConversant.DialogueUpdated -= UpdateUI;
        }

        private void UpdateUI()
        {
            gameObject.SetActive(_playerConversant.IsActive);
            if (_playerConversant.IsActive == false) return;

            _npcTextRoot.gameObject.SetActive(!_playerConversant.IsChoosing);
            _choiceRoot.gameObject.SetActive(_playerConversant.IsChoosing);

            if (_playerConversant.IsChoosing)
            {
                BuildAnswerOptions();
            }
            else
            {
                ChangeNpcPhrase();
            }
        }

        private void ChangeNpcPhrase()
        {
            _speakersName.text = _playerConversant.GetSpeakersName();
            _text.text = _playerConversant.GetText();
            _continueButton.gameObject.SetActive(_playerConversant.TryGetNext());
        }

        private void BuildAnswerOptions()
        {
            if (_choiceRoot.childCount > 0)
            {
                foreach (Transform child in _choiceRoot)
                {
                    Destroy(child.gameObject);
                }
            }

            foreach (DialogueNode chosenNode in _playerConversant.GetAnswerChoices())
            {
                Button newChoiceButton = Instantiate(_choiceButton, _choiceRoot);
                TMP_Text answer = newChoiceButton.GetComponentInChildren<TMP_Text>();
                answer.text = chosenNode.Text;
                _speakersName.text = chosenNode.SpeakerName;
                newChoiceButton.onClick.AddListener(() =>
                {
                    _playerConversant.SelectAnswerNode(newChoiceButton, chosenNode);
                });
            }
        }
    }
}