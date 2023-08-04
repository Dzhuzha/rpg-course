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
                    UpdateUI();
                });
            }
        }

        private void Close()
        {
            gameObject.SetActive(false);
        }
    }
}