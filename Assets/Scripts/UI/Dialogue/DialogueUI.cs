using UnityEngine;
using RPG.Dialogue;
using TMPro;

namespace RPG.UI
{
    public class DialogueUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        private PlayerConversant _playerConversant;

        private void Awake()
        {
            _playerConversant = FindObjectOfType<PlayerConversant>();
        }

        private void Start()
        {
            _text.text = _playerConversant.GetText();
        }
    }
}