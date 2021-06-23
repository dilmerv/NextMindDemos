using System.Collections;
using UnityEngine;

namespace NextMind.Examples.Calibration
{
    /// <summary>
    /// <see cref="FlashController"/> exposes convenient animation functions triggered on NeuroTags during calibration.
    /// </summary>
    public class FlashController : MonoBehaviour
    {
        [SerializeField]
        private Animator flashAnimator = null;
        internal Animator FlashAnimator => flashAnimator;

        internal IEnumerator FlashCoroutine()
        {
            yield return StartAndWait(flashAnimator, "Flash");
        }

        /// <summary>
        /// Convenient Coroutine used to wait for the animation state given in parameter.
        /// </summary>
        /// <param name="animator">The Animator to use</param>
        /// <param name="stateName">The state to start</param>
        private IEnumerator StartAndWait(Animator animator, string stateName)
        {
            animator.Play(stateName);

            // Wait for the state to be started
            yield return new WaitWhile(() => !animator.GetCurrentAnimatorStateInfo(0).IsName(stateName));

            // Wait for the end of the animation
            while (animator.IsInTransition(0) || animator.GetCurrentAnimatorStateInfo(0).IsName(stateName))
            {
                yield return null;
            }
        }
    }
}