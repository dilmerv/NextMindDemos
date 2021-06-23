using NextMind.Calibration;
using NextMind.Examples.Steps;
using NextMind.Devices;
using UnityEngine;
using UnityEngine.UI;

namespace NextMind.Examples.Calibration
{
    /// <summary>
    /// Implementation of an <see cref="AbstractStep"/> managed by the <see cref="StepsManager"/>.
    /// During this step, the calibration results are shown to the user. It is managing the loading, and eventually the error messages.
    /// </summary>
    public class ResultStep : AbstractStep
    {
        [SerializeField]
        private Text score = null;

        [SerializeField]
        protected GameObject nextButton = null;
        [SerializeField]
        protected GameObject backButton = null;

        /// <summary>
        /// Array of the grade disks.
        /// </summary>
        [SerializeField]
        private Image[] gradeImage = null;

        /// <summary>
        /// Array of the mentions.
        /// </summary>
        [SerializeField,TextArea]
        private string[] gradeMention = null;

        public override void OnEnterStep()
        {
            // Force button being in a close state.
            Animator animator = backButton.GetComponent<Animator>();
            animator.SetBool("IsOpened", false);
            animator.Play("Close", 0, 1);

            // Set the grade images to the default size and color.
            for (int i = 0; i < gradeImage.Length; i++)
            {
                SetGrade(i, 0.25f, Vector3.one);
            }

            Device connectedDevice = NeuroManager.Instance.ConnectedDevices[0];
            DisplayResults(connectedDevice.GetCalibrationResults().Grade);
        }

        public override void OnExitStep()
        {
            base.OnExitStep();

            backButton.GetComponent<Animator>().SetBool("IsOpened", false);
        }

        private void DisplayResults(CalibrationResults.CalibrationGrade grade)
        {
            // Display the score of the calibration.
            score.text = gradeMention[(int)grade];
            SetGrade((int)grade, 1, 1.2f * Vector3.one);
        }

        private void SetGrade(int index, float alpha, Vector3 localScale)
        {
            Image image = gradeImage[index];

            Color newColor = image.color;
            newColor.a = alpha;

            image.color = newColor;
            image.transform.localScale = localScale;
        }

        /// <summary>
        /// The back button displays a confirmation message on the first click.
        /// Manage this behaviour and return to the beginning when a second clic happen.
        /// </summary>
        public void OnClickOnBackButton()
        {
            bool opened = backButton.GetComponent<Animator>().GetBool("IsOpened");

            if (opened)
            {
                stepsManager.GoToStep(4);
            }
            else
            {
                backButton.GetComponent<Animator>().SetBool("IsOpened",true);
            }
        }
    }
}