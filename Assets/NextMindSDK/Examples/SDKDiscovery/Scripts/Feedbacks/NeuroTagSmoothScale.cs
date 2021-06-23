using System.Collections;
using UnityEngine;

namespace NextMind.Examples.Feedbacks
{
    /// <summary>
    /// The NeuroTagSmoothScale component is meant to be used with a NeuroTag.
    /// It makes the transform of this instance scale up on trigger and scale back to its original size on release of a NeuroTag.
    /// </summary>
    public class NeuroTagSmoothScale : MonoBehaviour
    {
        /// <summary>
        /// The scale used when NeuroTags is triggered.
        /// </summary>
        [SerializeField]
        private Vector3 targetScale = Vector3.one;
        /// <summary>
        /// The scale used when NeuroTags is released.
        /// </summary>
        private Vector3 originalScale;

        [SerializeField]
        private AnimationCurve curve = null;

        [SerializeField]
        private float animationDuration = 0.25f;

        private Coroutine scaleCoroutine;

        #region Unity Methods

        private void Start()
        {
            originalScale = transform.localScale;
        }

        #endregion

        #region NeuroTags events

        public void OnTriggered()
        {
            // If the coroutine is already running, stop it first.
            if (scaleCoroutine != null)
            {
                StopCoroutine(scaleCoroutine);
            }
            scaleCoroutine = StartCoroutine(SmoothScale(targetScale, animationDuration));
        }

        public void OnReleased()
        {
            // If the coroutine is already running, stop it first.
            if (scaleCoroutine != null)
            {
                StopCoroutine(scaleCoroutine);
            }
            scaleCoroutine = StartCoroutine(SmoothScale(originalScale, animationDuration));
        }

        #endregion

        private IEnumerator SmoothScale(Vector3 targetscale, float duration)
        {
            Vector3 originScale = transform.localScale;
            float t = 0;
            float currentTime = 0;

            while (t <= 1)
            {
                yield return new WaitForFixedUpdate();
                transform.localScale = Vector3.LerpUnclamped(originScale, targetscale, curve.Evaluate(t));

                currentTime += Time.fixedDeltaTime;
                t = currentTime / duration;
            }

            transform.localScale = targetscale;
            scaleCoroutine = null;
        }

    }
}