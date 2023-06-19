using System.Collections;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class Fader : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;

        private const byte NO_OPACITY = 1;
        private const byte FULL_OPACITY = 0;

        public IEnumerator FadeOut(float time)
        {
            while (_canvasGroup.alpha < NO_OPACITY)
            {
                _canvasGroup.alpha += Time.deltaTime / time;
                yield return null;
            }
        }

        public IEnumerator FadeIn(float time)
        {
            _canvasGroup.alpha = NO_OPACITY;

            while (_canvasGroup.alpha > float.Epsilon)
            {
                _canvasGroup.alpha -= Time.deltaTime / time;
                yield return null;
            }

            _canvasGroup.alpha = FULL_OPACITY;
        }
    }
}