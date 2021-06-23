using NextMind.Examples.Steps;
using System.Collections;
using UnityEngine;

namespace NextMind.Examples.Discovery
{
    /// <summary>
    /// Implementation of an <see cref="AbstractStep"/> managed by the <see cref="StepsManager"/>.
    /// During this step, the user can apply a force on a cube by focusing on it.
    /// </summary>
    public class DiscoveryStep : AbstractStep
    {
        /// <summary>
        /// The cube on which apply the force.
        /// </summary>
        [SerializeField]
        private Rigidbody rigidBody = null;

        /// <summary>
        /// The current force to apply on the cube.
        /// </summary>
        private float strength;

        #region AbstractStep implementation

        public override void OnEnterStep()
        {
            // Place the cube at the initial position each time this step is activated.
            StartCoroutine(SetToInitialPosition());
        }

        public override void UpdateStep()
        {
            // Apply the force.
            rigidBody.AddForce(Vector3.forward * Time.deltaTime * 15f * strength, ForceMode.Force);
        }

        #endregion

        #region NeuroTag events

        /// <summary>
        /// Update the strength to apply with received confidence value.
        /// </summary>
        /// <param name="value">The confidence value</param>
        public void OnConfidenceChanged(float value)
        {
            this.strength = value;
        }

        #endregion

        /// <summary>
        /// If the cube enter the collider attached to this instance, put back the cube to its initial position.
        /// Happens when the cube has been pushed to far away.
        /// </summary>
        /// <param name="collision">The zone limit trigger</param>
        private void OnTriggerEnter(Collider collision)
        {
            StartCoroutine(SetToInitialPosition(5f,2f));
        }

        private IEnumerator SetToInitialPosition(float height = 0.5f, float delay = 0f)
        {
            if (delay > 0)
            {
                yield return new WaitForSeconds(delay);
            }

            // Reset rigidbody's physics values.
            strength = 0;
            rigidBody.velocity = Vector3.zero;
            rigidBody.angularVelocity = Vector3.zero;

            // Reset transform to the right position.
            rigidBody.transform.localPosition = Vector3.up * height;
            rigidBody.transform.localEulerAngles = 5 * Vector3.one;
        }
    }
}