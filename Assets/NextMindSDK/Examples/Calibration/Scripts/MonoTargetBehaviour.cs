using NextMind.Calibration;
using NextMind.NeuroTags;
using System.Collections;

namespace NextMind.Examples.Calibration
{
    /// <summary>
    /// The NeuroTag behaviour used during the MonoTarget calibration.
    /// </summary>
    public class MonoTargetBehaviour : TagCalibrationBehaviour
    {
        /// <summary>
        /// The calibration step.
        /// </summary>
        private MonoTargetCalibrationStep monoTargetStep;

        public void InitFromStep(MonoTargetCalibrationStep step)
        {
            monoTargetStep = step;
        }

        /// <summary>
        /// Initialization of the NeuroTags at the very start of the calibration.
        /// </summary>
        /// <param name="tag">The NeuroTag to initialize.</param>
        public override void OnInitialize(NeuroTag tag)
        {
        }

        /// <summary>
        /// This is just needed to hide the loading glow and show the Neurotag when calibration is beginning.
        /// </summary>
        /// <param name="tag">The NeuroTag used during the trial which will start.</param>
        public override IEnumerator OnStartCalibrating(NeuroTag tag)
        {
            yield return monoTargetStep.StartCoroutine(monoTargetStep.OnTrialBegin(tag));
        }

        /// <summary>
        /// When a trial is over, warn the monoTargetStep
        /// </summary>
        /// <param name="tag">The NeuroTag used during the trial which just ends.</param>
        public override IEnumerator OnEndCalibrating(NeuroTag tag)
        {
            yield return monoTargetStep.StartCoroutine(monoTargetStep.OnTrialOver(tag));
        }
    }
}