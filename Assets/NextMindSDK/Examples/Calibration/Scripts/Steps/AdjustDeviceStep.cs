using NextMind.Devices;
using NextMind.Examples.Steps;
using NextMind.Examples.Utility;
using UnityEngine;
using UnityEngine.Video;

namespace NextMind.Examples.Calibration
{
    /// <summary>
    /// Implementation of an <see cref="AbstractStep"/> managed by the <see cref="StepsManager"/>.
    /// During this step, the user has to adjust the sensor on his head. It displays a validation message when the values are reliable enough.
    /// </summary>
    public class AdjustDeviceStep : AbstractStep
    {
        [SerializeField]
        private CanvasFader troubleshootingCanvasFader;
        [SerializeField]
        private VideoPlayer troubleshootingVideoPlayer;

        [SerializeField]
        private ContactQualitySlider contactQualitySlider = null;
        [SerializeField]
        private ContactQualitySlider helpPopupContactQualitySlider = null;

        private readonly float timeBeforeHelpDisplay = 30f;
        private float helpCurrentTimer = 0f;

        #region AbstractStep implementation

        public override void UpdateStep()
        {
            // Show the help part if the average value remain under Good during timeBeforeHelpDisplay seconds.
            var neuroManager = NeuroManager.Instance;
            if (neuroManager.ConnectedDevices.Count > 0)
            {
                Device connectedDevice = neuroManager.ConnectedDevices[0];

                var averageValue = GetGlobalContactScore(connectedDevice);

                contactQualitySlider.CurrentGlobalScore = averageValue;
                helpPopupContactQualitySlider.CurrentGlobalScore = averageValue;

                // If the troubleshooting canvas is already visible, stop here
                if (troubleshootingCanvasFader.IsCanvasVisible)
                {
                    return;
                }

                switch (averageValue)
                {
                    case ContactScore.NOT_WORN:
                    case ContactScore.WEAK:
                    case ContactScore.MEDIUM:
                        helpCurrentTimer += Time.deltaTime;

                        if (helpCurrentTimer > timeBeforeHelpDisplay)
                        {
                            troubleshootingCanvasFader.gameObject.SetActive(true);
                        }

                        break;

                    case ContactScore.GOOD:
                    case ContactScore.EXCELLENT:
                        helpCurrentTimer = 0;
                      
                        break;
                }
            }
        }

        public void OnCloseTroubleshootingPanel()
        {
            helpCurrentTimer = 0f;

            troubleshootingVideoPlayer.Stop();
            troubleshootingVideoPlayer.targetTexture.Release();

            troubleshootingCanvasFader.StartFade(false);
        }

        /// <summary>
        /// Reset values on exiting step.
        /// </summary>
        public override void OnExitStep()
        {
            helpCurrentTimer = 0f;

            if (troubleshootingCanvasFader.IsCanvasVisible)
            {
                troubleshootingCanvasFader.StartFade(false, true);
            }
        }

        #endregion

        internal enum ContactScore
        {
            NOT_WORN,
            WEAK,
            MEDIUM,
            GOOD,
            EXCELLENT
        }

        private ContactScore GetGlobalContactScore(Device connectedDevice)
        {
            float average = 0;
            int electrodesNumber = 8;

            for (int i = 0; i < electrodesNumber; i++)
            {
                var value = connectedDevice.GetContact((uint)i);
                if (value < 0 || value > 100)
                {
                    value = 0;
                }

                average += value;
            }
            average /= electrodesNumber;

            ContactScore globalScore;

            if (average < 5f)
            {
                globalScore = ContactScore.NOT_WORN;
            }
            else if (average < 30f)
            {
                globalScore = ContactScore.WEAK;
            }
            else if (average < 70f)
            {
                globalScore = ContactScore.MEDIUM;
            }
            else if (average < 90f)
            {
                globalScore = ContactScore.GOOD;
            }
            else //if (average <= 1f)
            {
                globalScore = ContactScore.EXCELLENT;
            }

            return globalScore;
        }
    }
}