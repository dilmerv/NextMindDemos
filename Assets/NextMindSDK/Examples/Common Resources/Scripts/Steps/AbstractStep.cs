using UnityEngine;

namespace NextMind.Examples.Steps
{
    /// <summary>
    /// An AbstractStep is the base class defining a step managed by the <see cref="StepsManager"/>.
    /// </summary>
    public abstract class AbstractStep : MonoBehaviour
    {
        /// <summary>
        /// A reference to the stepsManager.
        /// </summary>
        protected internal StepsManager stepsManager;

        /// <summary>
        /// Function triggered when this step instance is opened.
        /// </summary>
        public virtual void OnEnterStep() { }

        /// <summary>
        /// Function triggered when this step instance is left.
        /// </summary>
        public virtual void OnExitStep() { }

        /// <summary>
        /// Function triggered every frame when this step instance is active.
        /// </summary>
        public virtual void UpdateStep() { }

        /// <summary>
        /// Function telling the StepsManager if going to the next step from this one is allowed.
        /// </summary>
        /// <returns>True by default.</returns>
        public virtual bool GoToNextStepAllowed() { return true; }

        /// <summary>
        /// Function telling the StepsManager if going back to the previous step from this one is allowed.
        /// </summary>
        /// <returns>True by default.</returns>
        public virtual bool GoToPreviousStepAllowed() { return true; }
    }
}