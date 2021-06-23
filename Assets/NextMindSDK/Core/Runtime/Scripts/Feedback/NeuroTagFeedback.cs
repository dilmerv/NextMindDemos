using UnityEngine;

namespace NextMind.NeuroTags
{
    /// <summary>
    /// A simple component forwarding the last confidence values of a <see cref="NeuroTag"/> to the <see cref="Animator"/> attached to it.
    /// </summary>
    /// <remarks>If the <see cref="neuroTag"/> is not referenced, this component will be disabled.</remarks>
    [RequireComponent(typeof(Animator))]
    public class NeuroTagFeedback : MonoBehaviour
    {
        /// <summary>
        /// The NeuroTag from which we monitor the confidence.
        /// </summary>
        /// <remarks>If empty, the first NeuroTag found in parents will be used.</remarks>
        [SerializeField]
        private NeuroTag neuroTag = null;

        /// <summary>
        /// Should the feedback use the raw value received or should it rather be smoothed ?
        /// </summary>
        [SerializeField]
        private bool interpolateConfidenceValue = true;

        /// <summary>
        /// The speed of interpolation in case we interpolate the confidence value.
        /// </summary>
        [SerializeField]
        private float confidenceSmoothingSpeed = 5;

        /// <summary>
        /// The animator on which the confidence values will be forwarded.
        /// </summary>
        protected Animator animator;

        /// <summary>
        /// The current value.
        /// </summary>
        private float currentConfidenceValue = 0;
        /// <summary>
        /// The targeted value.
        /// </summary>
        private float targetConfidenceValue = 0;

        /// <summary>
        /// The name of the float parameter in the animator.
        /// </summary>
        private const string confidenceParameterName = "ConfidenceValue";

        #region Unity Methods

        private void Awake()
        {
            // Find the animator component on this GameObject instance.
            animator = GetComponent<Animator>();
            
            if (neuroTag == null)
            {
                // Find the NeuroTag component in parents
                neuroTag = GetComponentInParent<NeuroTag>();
            }

            if (neuroTag != null)
            {
                neuroTag.onConfidenceChanged.AddListener(OnConfidenceUpdated);
            }
        }

        private void Update()
        {
            HandleConfidenceUpdate();
        }

        private void OnDestroy()
        {
            if (neuroTag != null)
            {
                neuroTag.onConfidenceChanged.RemoveListener(OnConfidenceUpdated);
            }
        }

        private void OnEnable()
        {
            // Disable this component if no NeuroTag is found.
            if (neuroTag == null)
            {
                Debug.LogWarning("This feedback must be linked to a NeuroTag.", this);
                this.enabled = false;
            }
        }

        #endregion

        /// <summary>
        /// Interpolate the confidence value and forward it to the animator.
        /// </summary>
        private void HandleConfidenceUpdate()
        {
            if (interpolateConfidenceValue)
            {
                currentConfidenceValue = Mathf.Lerp(currentConfidenceValue, targetConfidenceValue, confidenceSmoothingSpeed * Time.deltaTime);
            }
            else
            {
                currentConfidenceValue = targetConfidenceValue;
            }

            animator.SetFloat(confidenceParameterName, currentConfidenceValue);
        }

        /// <summary>
        /// Update the targeted value.
        /// </summary>
        private void OnConfidenceUpdated(float value)
        {
            targetConfidenceValue = value;
        }
    }
}