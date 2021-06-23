using NextMind.Examples.Steps;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace NextMind.Examples.Discovery
{
    /// <summary>
    /// Implementation of an <see cref="AbstractStep"/> managed by the <see cref="StepsManager"/>.
    /// During this step, the user learn the usage of the OnTriggered NeuroTags actions.
    /// </summary>
    public class PersistantToggleStep : AbstractStep
    {
        /// <summary>
        /// The spot light reacting to the NeuroTag's trigger.
        /// </summary>
        [SerializeField]
        private GameObject spotLight = null;
        /// <summary>
        /// The animator attached to the NeuroTag 
        /// </summary>
        [SerializeField]
        private Animator toggleAnimator = null;

        /// <summary>
        /// The image component of the toggle's knob.
        /// </summary>
        [SerializeField]
        private Image knob = null;
        /// <summary>
        /// The color to apply on the nob when untoggled.
        /// </summary>
        [SerializeField]
        private Color knobOffColor = Color.grey;
        /// <summary>
        /// The color to apply on the nob when toggled.
        /// </summary>
        [SerializeField]
        private Color knobOnColor = Color.green;

        private bool triggered = false;
        private float triggerTime = 0;

        #region AbstractStep implementation

        public override void OnEnterStep()
        {
            // Dim the lights when entering the step, to better see the action on the lights.
            StartCoroutine(HubManager.Instance.SetLights(false));

            spotLight.SetActive(false);

            // Ensure having the toggle turned off.
            triggered = false;
            Toggle(false,true);
        }

        public override void OnExitStep()
        {
            // Set the lights intensities back to their original values.
            HubManager.Instance.StartCoroutine(HubManager.Instance.SetLights(true));

            base.OnExitStep();
        }

        #endregion

        #region NeuroTag events

        /// <summary>
        /// Function triggered when user focus on the toggle.
        /// </summary>
        public void OnTriggered()
        {
            Toggle(!triggered);

            triggerTime = Time.time;
        }

        /// <summary>
        /// Function triggered when user maintain the focus on the toggle.
        /// </summary>
        public void OnMaintained()
        {
            // If the user maintained his focus for more than 4 seconds, toggle the button again.
            if (Time.time - triggerTime > 4)
            {
                OnTriggered();
            }
        }

        #endregion

        /// <summary>
        /// Set the state of the toggle.
        /// </summary>
        /// <param name="toggled"></param>
        /// <param name="instant">If true, skip the animation</param>
        private void Toggle(bool toggled, bool instant=false)
        {
            triggered = toggled;

            // Animate the knob.
            toggleAnimator.SetBool("Toggled", triggered);

            if (instant)
            {
                toggleAnimator.Play("SwitchOff", 0, 1);
                knob.material.color = triggered ? knobOnColor : knobOffColor;
            }
            else
            {
                // Change the color.
                StartCoroutine(SwitchColor(triggered ? knobOnColor : knobOffColor));
            }

            // Set the spotlight on/off regarding the toggle value.
            spotLight.SetActive(triggered);
        }

        /// <summary>
        /// Smoothly change the color of toggle's knob.
        /// </summary>
        /// <param name="targetColor">The targeted color</param>
        private IEnumerator SwitchColor(Color targetColor)
        {
            float t = 0;
            float currentTime = 0f;
            float duration = 0.5f;

            Color startColor = knob.material.color;

            while (t < 1)
            {
                Color c = Color.Lerp(startColor, targetColor, t);
                knob.material.color = c;

                currentTime += Time.deltaTime;
                t = currentTime / duration;

                yield return null;
            }

            knob.material.color = targetColor;
        }
    }
}