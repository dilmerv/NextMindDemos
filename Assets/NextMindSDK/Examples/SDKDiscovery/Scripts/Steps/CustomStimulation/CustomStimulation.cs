using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace NextMind.Examples.Discovery
{
    public class CustomStimulation : MonoBehaviour
    {
        [SerializeField]
        private Sprite stimulationOff = null;
        [SerializeField]
        private Sprite stimulationOn = null;

        [SerializeField]
        private Image image = null;

        [SerializeField]
        private RectTransform selectionFeedback = null;

        #region NeuroTag events

        public void OnStimulationStateUpdated(GameObject neuroTag, float stimulationValue)
        {
            if (stimulationValue > 0.5f)
            {
                image.sprite = stimulationOn;
            }
            else
            {
                image.sprite = stimulationOff;
            }
        }

        public void OnTrigger()
        {
            var feedbackPosition = selectionFeedback.anchoredPosition;
            feedbackPosition.y = image.rectTransform.anchoredPosition.y;

            selectionFeedback.gameObject.SetActive(true);

            StartCoroutine(SmoothMoveSelectionFeedback(feedbackPosition));
        }

        #endregion

        private IEnumerator SmoothMoveSelectionFeedback(Vector2 targetPosition)
        {
            Vector2 startPosition = selectionFeedback.anchoredPosition;
            float t = 0;
            float duration = 0.3f;
            float timer = 0f;

            AnimationCurve curve = AnimationCurve.EaseInOut(0,0,1,1);

            while (t < 1)
            {
                selectionFeedback.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, curve.Evaluate(t));

                timer += Time.deltaTime;

                t = timer / duration;

                yield return null;
            }

            selectionFeedback.anchoredPosition = targetPosition;
        }
    }
}