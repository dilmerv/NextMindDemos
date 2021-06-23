using UnityEngine;
using UnityEngine.UI;

namespace NextMind.Examples.Steps
{
    /// <summary>
    /// Implementation of an <see cref="AbstractStep"/> managed by the <see cref="StepsManager"/>.
    /// This step is used during the phases where it is needed to wait for the system to be ready (juste before calibration, or starting the demos).
    /// </summary>
    public class WaitingForSystemReadyStep : AbstractStep
    {
        /// <summary>
        /// Should we automatically go to the next step when system is ready ?
        /// </summary>
        [SerializeField]
        private bool autoStartNextStep = false;

        /// <summary>
        /// Moving part of the loading feedback.
        /// </summary>
        [SerializeField]
        private Image loadingBar = null;
        /// <summary>
        /// Fixed part of the loading feedback.
        /// </summary>
        [SerializeField]
        private Image loadingBackground = null;

        [SerializeField]
        private Button nextButton = null;
        /// <summary>
        /// The text element where to display the system-ready message.
        /// </summary>
        [SerializeField]
        private Text description = null;
        /// <summary>
        /// The sentence to display when system is ready.
        /// </summary>
        [SerializeField]
        private string readyDescription = null;
        
        private string originalDescription;

        public override void OnEnterStep()
        {
            if (NeuroManager.Instance.IsReady())
            {
                OnSystemReady();
            }
            else
            {
                // Store the original description.
                originalDescription = description.text;

                // Block nextButton interaction.
                nextButton.interactable = false;

                ShowLoading(true);
            }
        }

        public override void OnExitStep()
        {
            // Reset elements.
            description.text = originalDescription;
            nextButton.interactable = false;

            // Stop loading animation.
            ShowLoading(false);
        }

        public override void UpdateStep()
        {
            if (NeuroManager.Instance.IsReady())
            {
                OnSystemReady();
            }
        }

        private void OnSystemReady()
        {
            if (autoStartNextStep)
            {
                stepsManager.OnClickOnNextStep(true);
            }
            else
            {
                ShowLoading(false);
                description.text = readyDescription;
                nextButton.interactable = true;
            }
        }

        /// <summary>
        /// Start or stop the movement of the loading feedback.
        /// </summary>
        /// <param name="show">The wanted loading state</param>
        private void ShowLoading(bool show)
        {
            Color green = loadingBar.color;
            if (show)
            {
                green.a = 0.1f;
            }
            loadingBackground.color = green;

            loadingBar.gameObject.SetActive(show);
        }

        /// <inheritdoc />
        public override bool GoToNextStepAllowed()
        {
            return !autoStartNextStep && NeuroManager.Instance.IsReady();
        }

        /// <inheritdoc />
        public override bool GoToPreviousStepAllowed()
        {
            return false;
        }
    }
}