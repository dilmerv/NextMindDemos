using NextMind;
using NextMind.Devices;
using NextMind.Examples.Steps;
using UnityEngine;

namespace NextMind.Examples.Calibration
{
    /// <summary>
    /// Implementation of an <see cref="AbstractStep"/> managed by a <see cref="StepsManager"/>.
    /// This step is a StepsManager itself, handling substeps allowing users to pair/connect devices.
    /// </summary>
    public class DeviceConnectionStep : AbstractStep
    {
        /// <summary>
        /// The StepsManager handling all the connection and pairing substeps.
        /// </summary>
        [SerializeField]
        private StepsManager subStepsManager;

        /// <summary>
        /// Is the user currently navigating inside the pairing steps?
        /// </summary>
        private bool insidePairingSteps;

        #region AbstractStep implementation

        public override void OnEnterStep()
        {
            // Ensure starting from the first step.
            subStepsManager.Restart();
        }

        public override bool GoToNextStepAllowed()
        {
            return !insidePairingSteps && NeuroManager.Instance.ConnectedDevices.Count > 0;
        }

        public override bool GoToPreviousStepAllowed()
        {
            return !insidePairingSteps;
        }

        #endregion

        public void OnStartPairingSteps()
        {
            insidePairingSteps = true;
        }

        public void OnEndPairingSteps()
        {
            insidePairingSteps = false;
        }
    }
}