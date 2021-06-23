using UnityEngine;

namespace NextMind.Examples.Feedbacks
{
    /// <summary>
    /// Simple component used to trigger a particle burst from a given particle system.
    /// </summary>
    public class NeuroTagParticleBurst : MonoBehaviour
    {
        [SerializeField]
        private new ParticleSystem particleSystem = null;

        #region Unity Methods

        private void Start()
        {
            particleSystem.Stop();
        }

        #endregion

        #region NeuroTags events

        /// <summary>
        /// Play the particles burst when the user focus on the NeuroTag.
        /// Triggered by the NeuroTag.
        /// </summary>
        public void OnTriggered()
        {
            particleSystem.Play();
        }

        /// <summary>
        /// Stop the particles burst when the focus is released from the NeuroTag.
        /// Triggered by the NeuroTag.
        /// </summary>
        public void OnReleased()
        {
            particleSystem.Stop();
        }

        #endregion
    }
}