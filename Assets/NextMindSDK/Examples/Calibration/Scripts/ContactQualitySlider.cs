using UnityEngine;
using static NextMind.Examples.Calibration.AdjustDeviceStep;

namespace NextMind.Examples.Calibration
{
    public class ContactQualitySlider : MonoBehaviour
    {
        [SerializeField]
        private Animator globalQualityAnimator = null;

        private readonly float upwardSmoothTime = 0.5f;
        private readonly float downwardSmoothTime = 2f;
        private float velocity = 0.0f;

        internal ContactScore CurrentGlobalScore { get; set; }

        private void Update()
        {
            UpdateScoreSlider();
        }

        private void UpdateScoreSlider()
        {
            float currentQuality = globalQualityAnimator.GetFloat("Quality");
            float targetQuality = (float)CurrentGlobalScore / 4f;

            float newPosition = Mathf.SmoothDamp(currentQuality, targetQuality, ref velocity, (currentQuality > targetQuality) ? downwardSmoothTime : upwardSmoothTime);

            globalQualityAnimator.SetFloat("Quality", newPosition);
        }
    }
}