using UnityEngine;
using UnityEngine.UI;
using NextMind.Calibration;
using NextMind.Devices;
using NextMind.Examples.Steps;

namespace NextMind.Examples.Calibration
{
    /// <summary>
    /// Implementation of an <see cref="AbstractStep"/> managed by the <see cref="StepsManager"/>.
    /// This step is used during the phases where it is needed to wait for the system to be ready (juste before calibration, or starting the demos).
    /// </summary>
    public class WaitingForCalibrationResultsStep : AbstractStep
    {
        /// <summary>
        /// A reference to the CalibrationManager.
        /// </summary>
        [SerializeField]
        private CalibrationManager calibrationManager = null;

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
        
        private string originalDescription = default;

        public override void OnEnterStep()
        {
            // Store the original description text.
            originalDescription = description.text;

            // If results are already available when entering the step, display them, else listen to the onCalibrationResultsAvailable event.
            if (calibrationManager.CalibrationResultsAvailable)
            {
                OnResultsReceived(null,default);
            }
            else
            {
                // Start loading animation.
                ShowLoading(true);

                nextButton.interactable = false;

                calibrationManager.onCalibrationResultsAvailable.AddListener(OnResultsReceived);
                calibrationManager.onCalibrationResultsError.AddListener(OnResultsError);
            }
        }

        public override void OnExitStep()
        {
            // Reset elements.
            description.text = originalDescription;
            nextButton.interactable = false;

            // Stop loading animation.
            ShowLoading(false);

            calibrationManager.onCalibrationResultsAvailable.RemoveListener(OnResultsReceived);
            calibrationManager.onCalibrationResultsError.RemoveListener(OnResultsError);
        }

        private void OnResultsReceived(Device device, CalibrationResults.CalibrationGrade results)
        {
            stepsManager.OnClickOnNextStep(true);
        }

        private void OnResultsError(string errorMessage)
        {
            DisplayError(errorMessage);
        }

        private void DisplayError(string message)
        {
            ShowLoading(false);

            nextButton.interactable = true;

            description.text = message;
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
            return false;
        }

        /// <inheritdoc />
        public override bool GoToPreviousStepAllowed()
        {
            return false;
        }
    }
}