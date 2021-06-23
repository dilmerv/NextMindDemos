using NextMind.Examples.Steps;
using UnityEngine;

namespace NextMind.Examples.Discovery
{
	/// <summary>
	/// Implementation of an <see cref="AbstractStep"/> managed by the <see cref="StepsManager"/>.
	/// During this step, the user learn the usage of OnReleased event.
	/// </summary>
	public class ToggleReleaseStep : AbstractStep
	{
		/// <summary>
		/// The NeuroTag on which the user has to focus. 
		/// </summary>
		[SerializeField]
		private Transform cube = null;

        #region AbstractStep implementation

        public override void OnEnterStep()
		{
			// Position the cube a little high, so the user can see it fall when entering the step.
			cube.localPosition = Vector3.up * 0.5f;
			cube.localEulerAngles = new Vector3(5, 5, 5);
		}

		#endregion
	}
}