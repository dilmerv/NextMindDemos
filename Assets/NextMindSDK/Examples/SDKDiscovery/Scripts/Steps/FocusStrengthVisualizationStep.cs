using NextMind.Examples.Steps;
using UnityEngine;

namespace NextMind.Examples.Discovery
{
	/// <summary>
	/// Implementation of an <see cref="AbstractStep"/> managed by the <see cref="StepsManager"/>.
	/// During this step, the user can see a particle system becoming stronger while he is more and more focusing on the NeuroTag. 
	/// </summary>
	public class FocusStrengthVisualizationStep  : AbstractStep
	{
		/// <summary>
		/// The particle system to modulate.
		/// </summary>
		[SerializeField]
		private new ParticleSystem particleSystem = null;
		/// <summary>
		/// The NeuroTag on which the user has to focus. 
		/// </summary>
		[SerializeField]
		private Transform cube = null;

		private float targetStrength = 0f;
		private float currentStrength = 0f;

        #region AbstractStep implementation

        public override void OnEnterStep()
		{
			// Position the cube a little high, so the user can see it fall when entering the step.
			cube.localPosition = Vector3.up * 0.5f;
			cube.localEulerAngles = new Vector3(5,5,5);

			// Set particle system to minimum values.
			ApplyStrength(0);
		}

		public override void UpdateStep()
		{
			if (!Mathf.Approximately(targetStrength, currentStrength))
			{
				currentStrength = Mathf.Lerp(currentStrength, targetStrength, Time.deltaTime);

				ApplyStrength(currentStrength);
			}
		}

		#endregion

		#region NeuroTag events

		/// <summary>
		/// Update the strength to apply with received confidence value.
		/// </summary>
		/// <param name="value">The confidence value</param>
		public void OnConfidenceChanged(float value)
		{
			this.targetStrength = value;
		}

		#endregion

		/// <summary>
		/// Modulate the particle system values regarding the <paramref name="strentghValue"/>.
		/// </summary>
		/// <param name="strentghValue"></param>
		public void ApplyStrength(float strentghValue)
		{
			strentghValue = strentghValue < 0.05f ? 0 : strentghValue;
			strentghValue = strentghValue > 0.95f ? 1 : strentghValue;

			float miniRateOverTime = 0, maxRateOverTime = 500;

			// Emit more particles on high strength values.
			var emissionModule = particleSystem.emission;
			emissionModule.rateOverTime = miniRateOverTime + strentghValue * (maxRateOverTime - miniRateOverTime);

			// Make particles noisy, vibrating, on high strength values.
			var noiseModule = particleSystem.noise;
			if (strentghValue > 0.5f)
			{
				float minScrollSpeed = 0.01f, maxScrollSpeed = 50;
				float minStrength = 0.1f, maxStrength = 0.5f;
				float t = ( strentghValue - 0.5f ) / 0.5f;

				noiseModule.enabled = true;

				noiseModule.scrollSpeed = Mathf.Lerp(minScrollSpeed, maxScrollSpeed, t);

				noiseModule.strengthX = Mathf.Lerp(minStrength, maxStrength, t);
				noiseModule.strengthY = Mathf.Lerp(minStrength, maxStrength, t);
			}
			else
			{
				noiseModule.enabled = false;
			}

			// Make particles rotate and go upward on very high strength values.
			var velocityModule = particleSystem.velocityOverLifetime;
			if (strentghValue > 0.9f)
			{
				float minOrbitalSpeed = 0f, maxOrbitalSpeed = 1f;
				float minZVelo = 0f, maxZVelo = -0.12f;
				float t = (strentghValue - 0.9f) / (1-0.9f);

				velocityModule.enabled = true;

				velocityModule.orbitalZ = Mathf.Lerp(minOrbitalSpeed, maxOrbitalSpeed, t);

				velocityModule.z = Mathf.Lerp(minZVelo, maxZVelo, t);
			}
			else
			{
				velocityModule.enabled = false;
			}
		}
	}
}