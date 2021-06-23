using NextMind.NeuroTags;
using UnityEngine;

namespace NextMind.Examples.Discovery
{
    /// <summary>
    /// This feedback component forward the confidence values from the linked NeuroTag to the <see cref="LevitationMovement"/> component. 
    /// In paralell, it manages the evolution of a particles system, regarding the same confidence values.
    /// </summary>
    [RequireComponent(typeof(LevitationMovement))]
    [RequireComponent(typeof(NeuroTag))]
    public class LevitatingNeuroTagFeedback : MonoBehaviour
    {
        [SerializeField]
        private ParticleSystem particles = null;
        [SerializeField]
        private bool useParticleSystem = false;

        private LevitationMovement movement;

        #region Unity methods

        void Start()
        {
            // Grab the LevitationMovement component.
            movement = GetComponent<LevitationMovement>();

            // Find and activate the particles system following the useParticleSystem value.
            if (particles != null)
            {
                particles.gameObject.SetActive(useParticleSystem);
            }
            else
            {
                // If not found, disable the particles usage.
                useParticleSystem = false;
            }
        }

        #endregion

        #region NeuroTag events

        /// <summary>
        /// Forward the confidence values to the particles system and the LevitationMovement component.
        /// </summary>
        public void OnConfidenceChange(float value)
        {
            movement.SetCurrentPosition(value);

            if (useParticleSystem)
            {
                UpdateParticleSystem(value);
            }
        }

        #endregion

        /// <summary>
        /// Update particles system's subsytems values.
        /// </summary>
        /// <param name="strentghValue">the value between 0 and 1</param>
        private void UpdateParticleSystem(float strentghValue)
        {
            // Make the particles faster in case of a higher strengh.
            var velo = particles.velocityOverLifetime;
            velo.radial =  - 1.25f * strentghValue;
            velo.speedModifierMultiplier = 0.005f + 0.1f * strentghValue;

            // More particles in case of a higher strengh.
            float miniRateOverTime = 0, maxRateOverTime = 150;
            var emissionModule = particles.emission;
            emissionModule.rateOverTime = miniRateOverTime + strentghValue * (maxRateOverTime - miniRateOverTime);
        }
    }
}