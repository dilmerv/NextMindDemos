using UnityEngine;

namespace NextMind.Examples.Discovery
{
    /// <summary>
    /// This component add a levitation effect to an object. 
    /// It make it moves smoothly on its y axis following the given <see cref="amplitude"/> and <see cref="frequency"/>.
    /// Moreover, the Y value around which this levitation effect happen can be set using <see cref="SetCurrentPosition"/>. 
    /// </summary>
    public class LevitationMovement : MonoBehaviour
    {
        [SerializeField]
        private Transform hostTransform = null;

        /// <summary>
        /// The levitation effect frequency.
        /// </summary>
        [SerializeField]
        private float frequency = 1f;
        /// <summary>
        /// The levitation effect amplitude.
        /// </summary>
        [SerializeField]
        private float amplitude = 0.02f;

        /// <summary>
        /// The distance between lowerY and upperY.
        /// </summary>
        [SerializeField]
        private float distanceBetweenPosisiton = 0.3f;

        /// <summary>
        /// Should we add a random rotation?
        /// </summary>
        [SerializeField]
        private bool rotateRandomly = true;

        /// <summary>
        /// The Y value to apply for SetCurrentPosition(0)
        /// </summary>
        private float lowerY;
        /// <summary>
        /// The Y value to apply for SetCurrentPosition(1)
        /// </summary>
        internal float upperY;
        /// <summary>
        /// The current Y value.
        /// </summary>
        private float targetY;

        /// <summary>
        /// The animation curve used to evaluate how the transition between lowerY and upperY is done.
        /// </summary>
        private AnimationCurve positionTransitionCurve;

        /// <summary>
        /// The random rotation euler angles.
        /// </summary>
        private Vector3 randomRotation;

        /// <summary>
        /// The current Y position between lowerY and upperY.
        /// </summary>
        private float currentWideOscillationY;

        /// <summary>
        /// The timer used to modulate the amplitude.
        /// </summary>
        private float amplitudeTimer = 0f;

        private void Start()
        {
            if (hostTransform == null)
            {
                hostTransform = this.transform;
            }

            positionTransitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

            // Compute the limit values
            lowerY = hostTransform.position.y;
            upperY = lowerY + distanceBetweenPosisiton;

            currentWideOscillationY = lowerY;

            SetCurrentPosition(0);

            randomRotation = 10 * new Vector3(0, Random.value - 0.5f, 0);
        }

        void Update()
        {
            var newPosition = hostTransform.position;

            // Lerp the position between lowerY and upperY.
            currentWideOscillationY = Mathf.Lerp(currentWideOscillationY, targetY, 10 * Time.deltaTime);
            
            // Add the levitation effect.
            newPosition.y = currentWideOscillationY + amplitude * Mathf.Sin(frequency * amplitudeTimer);

            amplitudeTimer += Time.deltaTime;

            // Finally set the position.
            hostTransform.position = newPosition;

            // Add a slight random rotation if needed.
            if (rotateRandomly)
            {
                hostTransform.Rotate(randomRotation * Time.deltaTime);
            }
        }

        /// <summary>
        /// Define the wanted position between lowerY and upperY.
        /// </summary>
        /// <param name="t">The value between 0 (lowerY) and 1 (upperY)</param>
        public void SetCurrentPosition(float t)
        {
            targetY = Mathf.Lerp(lowerY, upperY, positionTransitionCurve.Evaluate(t));
        }
    }
}