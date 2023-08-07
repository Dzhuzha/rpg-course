using RPG.Control;
using UnityEngine;

namespace RPG.Dialogue
{
    public class NPCConversant : MonoBehaviour, IRaycastable
    {
        [SerializeField] private Dialogue _dialogue;

        public bool HandleRaycast(PlayerController callingController)
        {
            if (!callingController.transform.TryGetComponent(out PlayerConversant playerConversant)) return false;

            if (Input.GetMouseButton(0) && _dialogue != null)
            {
                playerConversant.BeginDialogue(this, _dialogue);
            }

            return true;
        }

        public CursorType GetCursorType()
        {
            return CursorType.Dialogue;
        }
    }
}