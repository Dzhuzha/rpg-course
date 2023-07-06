using System.Collections;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class Fader : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;

        private const byte NO_OPACITY = 1;
        private const byte FULL_OPACITY = 0;
        public bool IsFading { get; private set; } = false;

        public void FadeOutImmediate()
        {
            _canvasGroup.alpha = 1;
        }
        
        public IEnumerator FadeOut(float time)
        {
            while (_canvasGroup.alpha < NO_OPACITY)
            {
                _canvasGroup.alpha += Time.deltaTime / time;
                IsFading = true;
                yield return null;
            }
            
            IsFading = false;
        }

        public IEnumerator FadeIn(float time)
        {
            _canvasGroup.alpha = NO_OPACITY;

            while (_canvasGroup.alpha > float.Epsilon)
            {
                _canvasGroup.alpha -= Time.deltaTime / time;
                IsFading = true;
                yield return null;
            }

            IsFading = false;
            _canvasGroup.alpha = FULL_OPACITY;
        }
    }
}