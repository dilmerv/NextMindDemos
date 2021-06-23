using NextMind.Examples.Steps;
using UnityEngine;

namespace NextMind.Examples.Discovery
{
    public class AnimationStep : AbstractStep
    {
        [SerializeField]
        private Animator animator = null;

        #region AbstractStep implementation

        public override void OnEnterStep()
        {
            base.OnEnterStep();

            // Ensure the animator to be stopped.
            animator.speed = 0;
        }

        #endregion
    }
}