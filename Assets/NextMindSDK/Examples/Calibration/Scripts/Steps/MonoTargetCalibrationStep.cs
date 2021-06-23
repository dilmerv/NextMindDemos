using NextMind.Calibration;
using NextMind.Examples.Steps;
using NextMind.Examples.Utility;
using NextMind.NeuroTags;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace NextMind.Examples.Calibration
{
    /// <summary>
    /// Implementation of an <see cref="AbstractStep"/> managed by the <see cref="StepsManager"/>.
    /// This step manages a particular calibration called MonoTarget calibration. 
    /// This type of calibration is using only one NeuroTag during all the calibration process.
    /// It also give hints about user's progress in different ways: a fancy particle system displayed in background, and a circle growing step by step around the NeuroTag.
    /// </summary>
    public class MonoTargetCalibrationStep : AbstractStep
    {
        /// <summary>
        /// A reference to the CalibrationManager.
        /// </summary>
        [SerializeField]
        private CalibrationManager calibrationManager = null;

        [Header("Particles")]

        /// <summary>
        /// The particle system in background, giving hints about user's progress.
        /// </summary>
        [SerializeField]
        private ParticleSystem particles = null;

        /// <summary>
        /// An animationCurve used to define the way the particle system evolve to the final step.
        /// </summary>
        [SerializeField]
        private AnimationCurve progressCurve = null;

        [Header("NeuroTag group")]

        /// <summary>
        /// The circle growing step by step around the NeuroTag.
        /// </summary>
        [SerializeField]
        private Image progressionCircleHint = null;

        /// <summary>
        /// The RectTransform containing the NeuroTag used for the calibration and the progressionCircleHint.
        /// </summary>
        [SerializeField]
        private RectTransform neuroTagGroup = null;

        /// <summary>
        /// Parent transform of object displayed before calibration
        /// </summary>
        [SerializeField]
        private CanvasFader preCalibrationGroup = null;

        [Header("Error handling")]

        [SerializeField]
        private GameObject errorPanel = null;
        [SerializeField]
        private Text errorDescription;

        /// <summary>
        /// The total number of calibration trials.
        /// </summary>
        private int numberOfTrials;

        private int currentStep = 0;

        private float originalSimulationSpeed;

        #region Unity methods

        private void Awake()
        {
            numberOfTrials = calibrationManager.NumberOfTrials;

            // Instantiate the right number of points on the circle.
            Transform pointPrefab = progressionCircleHint.transform.GetChild(0);
            for (int i = 1; i < numberOfTrials; ++i)
            {
                Instantiate(pointPrefab.gameObject, progressionCircleHint.transform);
            }

            // Store the original particles simulation speed.
            originalSimulationSpeed = particles.main.simulationSpeed;
        }

        #endregion

        #region AbstractStep implementation

        public override void OnEnterStep()
        {
            // Register the strategy used by the NeuroTags during the calibration.
            MonoTargetBehaviour behaviour = new MonoTargetBehaviour();
            behaviour.InitFromStep(this);
            calibrationManager.SetNeuroTagBehaviour(behaviour);

            // Ensure displaying the preClibrationGroup at calibration start.
            preCalibrationGroup.StartFade(true, true);

            // Reset the particle system progress & speed.
            UpdateParticleSystem(0);
            var main = particles.main;
            main.simulationSpeed = originalSimulationSpeed;

            calibrationManager.StartCalibration();

            calibrationManager.onCalibrationOver.AddListener(OnCalibrationOver);
            calibrationManager.onCalibrationError.AddListener(OnCalibrationError);
        }

        public override void OnExitStep()
        {
            currentStep = 0;
            progressionCircleHint.fillAmount = 0;

            // Hide points on circle.
            foreach (Transform t in progressionCircleHint.transform)
            {
                t.gameObject.SetActive(false);
            }

            // Hide the error panel which have been displayed.
            errorPanel.SetActive(false);

            calibrationManager.onCalibrationOver.RemoveListener(OnCalibrationOver);
            calibrationManager.onCalibrationError.RemoveListener(OnCalibrationError);
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

        #endregion

        #region TagBehaviourStrategy callbacks

        public IEnumerator OnTrialBegin(NeuroTag tag)
        {
            // If it is the first step, show the Neurotag first
            if (currentStep == 0)
            {
                yield return StartCoroutine(preCalibrationGroup.Fade(0));
                yield return StartCoroutine(ShowTransform(neuroTagGroup, true));
            }
        }

        /// <summary>
        /// This method is called by the <see cref="MonoTargetBehaviour"/> when a NeuroTag ends its trial. 
        /// It is updating the different hints to the incremented step, then make the NeuroTag flash,
        /// waiting for it to end before returning.
        /// </summary>
        /// <param name="tag">The tag which ended its trial</param>
        public IEnumerator OnTrialOver(NeuroTag tag)
        {
            currentStep++;

            UpdateParticleSystem(currentStep);

            // Make the breadcrump circle progress around the NeuroTag.
            StartCoroutine(SmoothProgression());

            // Accelerate the simulation speed of the particles system during the flash.
            var main = particles.main;
            float originalSpeed = main.simulationSpeed;

            StartCoroutine(SmoothSetSimulationSpeed(1.5f, 0.25f));

            yield return StartCoroutine(Flash(tag));

            if (currentStep == numberOfTrials)
            {
                // Accelerate the simulation
                StartCoroutine(SmoothSetSimulationSpeed(1f, 0.25f));
                // Hide the Neurotag
                yield return StartCoroutine(ShowTransform(neuroTagGroup, false));
            }
            else
            {
                // Set back the simulation speed of the particles system to its original speed.
                StartCoroutine(SmoothSetSimulationSpeed(originalSpeed, 0.15f));
            }
        }

        #endregion

        #region CalibrationManager callbacks

        private void OnCalibrationOver()
        {
            stepsManager.OnClickOnNextStep(true);
        }

        private void OnCalibrationError(string errorMessage)
        {
            StartCoroutine(CalibrationErrorCoroutine(errorMessage));
        }

        private IEnumerator CalibrationErrorCoroutine(string errorMessage)
        {
            // Hide the Neurotag
            yield return StartCoroutine(ShowTransform(neuroTagGroup, false));

            errorDescription.text = errorMessage;
            errorPanel.SetActive(true);
        }

        #endregion

        /// <summary>
        /// Smoothly change the particles simulation speed value.
        /// </summary>
        /// <param name="targetValue">The value to reach</param>
        /// <param name="duration">The time to reach the <paramref name="targetValue"/></param>
        /// <returns></returns>
        private IEnumerator SmoothSetSimulationSpeed(float targetValue, float duration)
        {
            var main = particles.main;

            float startSpeed = main.simulationSpeed;
            float targetSpeed = targetValue;
            float t = 0;
            float currentTimer = 0;

            while (t < 1)
            {
                main.simulationSpeed = Mathf.Lerp(startSpeed, targetSpeed, t);

                currentTimer += Time.deltaTime;
                t = currentTimer / duration;

                yield return null;
            }

            main.simulationSpeed = targetSpeed;
        }

        /// <summary>
        /// Start the flash animation on the <paramref name="tag"/> and wait the animation end before returning.
        /// </summary>
        /// <param name="tag">The tag to flash</param>
        private IEnumerator Flash(NeuroTag tag)
        {
            FlashController flashController = tag.GetComponent<FlashController>();
            if (flashController != null)
            {
                yield return NeuroManager.Instance.StartCoroutine(flashController.FlashCoroutine());
            }
        }

        /// <summary>
        /// Smoothly make the circle around the NeuroTag progress to the current step.
        /// </summary>
        private IEnumerator SmoothProgression()
        {
            float startProgression = progressionCircleHint.fillAmount;
            float targetProgression = (float)currentStep / numberOfTrials;
            float t = 0;
            float duration=0.5f, currentTimer=0;

            while (t < 1)
            {
                float smoothedProgression = Mathf.Lerp(startProgression, targetProgression, t);

                progressionCircleHint.fillAmount = smoothedProgression;

                currentTimer += Time.deltaTime;
                t = currentTimer / duration;
                yield return null;
            }

            progressionCircleHint.fillAmount = targetProgression;

            yield return DisplayPoint();
        }

        /// <summary>
        /// Short animation scaling quickly up and down the point corresponding to the current step.
        /// </summary>
        private IEnumerator DisplayPoint()
        {
            float targetProgression = (float)currentStep / numberOfTrials;

            // Get the right point to animate.
            Transform point;
            if (targetProgression < 1)
            {
                point = progressionCircleHint.transform.GetChild(currentStep);
            }
            else
            {
                point = progressionCircleHint.transform.GetChild(0);
            }
            point.gameObject.SetActive(true);

            Vector3 startScale = point.localScale;
            Vector3 maxScale = 2 * startScale;

            // A simple curve having its maximum for t=0.5
            Keyframe[] keys = new Keyframe[]
            {
                new Keyframe(0,0),
                new Keyframe(0.5f,1),
                new Keyframe(1,0)
            };
            AnimationCurve curve = new AnimationCurve(keys);

            float t = 0;
            float duration = 0.25f, currentTimer = 0;

            while (t < 1)
            {
                point.localScale = Vector3.Lerp(startScale, maxScale, curve.Evaluate(t));

                currentTimer += Time.deltaTime;
                t = currentTimer / duration;
                yield return null;
            }
        }

        /// <summary>
        /// Update the particle system value regarding the current step.
        /// </summary>
        /// <param name="trialNumber">The current step index</param>
        private void UpdateParticleSystem(int trialNumber)
        {
            float progress = trialNumber / (float)numberOfTrials;

            // Modulate the progression regarding the curve given in the editor.
            progress = progressCurve.Evaluate(progress);

            // Reduce more and more the noise strength up to no noise.
            var noise = particles.noise;
            noise.strength = new ParticleSystem.MinMaxCurve(0.15f * (1 - progress));
        }

        /// <summary>
        /// Scale up or down the given transform.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="show"></param>
        /// <returns></returns>
        private IEnumerator ShowTransform(Transform transform, bool show)
        {
            Vector3 startScale = transform.localScale;
            Vector3 targetScale = show ? Vector3.one : Vector3.zero;
            float t = 0;
            float duration = 0.5f, currentTimer = 0;

            while (t < 1)
            {
                transform.localScale = Vector3.Lerp(startScale,targetScale,t);

                currentTimer += Time.deltaTime;
                t = currentTimer / duration;

                yield return null;
            }

            transform.localScale = targetScale;
        }
    }
}