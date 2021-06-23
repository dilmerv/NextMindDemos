using UnityEngine;
using UnityEngine.Events;

namespace NextMind.Examples.Steps
{
	/// <summary>
	/// The StepsManager is keeping track of the known <see cref="AbstractStep"/>s and provide functions to move from one step to another 
	/// while triggering basic events as <see cref="AbstractStep.OnEnterStep"/>, <see cref="AbstractStep.OnExitStep"/>, etc...
	/// </summary>
	public class StepsManager : MonoBehaviour
	{
		#region Unity Methods

		private void Awake()
		{
			startStepIndex = Mathf.Clamp(startStepIndex, 0, steps.Length);

			Restart();
		}

		private void Update()
		{
			steps[currentStepIndex].UpdateStep();

			if (allowKeyboardNavigation)
			{
				if (Input.GetKeyUp(KeyCode.RightArrow))
				{
					OnClickOnNextStep();
				}
				else if (Input.GetKeyUp(KeyCode.LeftArrow))
				{
					OnClickOnPreviousStep();
				}
			}
		}

		#endregion

		#region Step switching management

		/// <summary>
		/// The list of steps to manage.
		/// </summary>
		[SerializeField]
		private AbstractStep[] steps = null;

		/// <summary>
		/// From which step to start?
		/// </summary>
		[SerializeField]
		private int startStepIndex;

		/// <summary>
		/// Action to trigger when the asking for next step while current step is already the last one.
		/// </summary>
		[SerializeField]
		private bool allowKeyboardNavigation = true;

		/// <summary>
		/// Action to trigger when the asking for next step while current step is already the last one.
		/// </summary>
		[SerializeField]
		private UnityEvent NextOnLastStepAction = null;
		
		/// <summary>
		/// Action to trigger when the asking for previous step while current step is already the first one.
		/// </summary>
		[SerializeField]
		private UnityEvent PreviousOnFirstStepAction = null;

		private bool IsFirstStep => currentStepIndex == 0;
		private bool IsLastStep => currentStepIndex >= steps.Length - 1;

		private int currentStepIndex = -1;
		
		public void InitializeSteps()
		{
			// Set the first step as the only active step.
			for (int i = 0; i < steps.Length; i++)
			{
				steps[i].gameObject.SetActive(i == startStepIndex);
				steps[i].stepsManager = this;
			}
		}

        public void Restart()
        {
			InitializeSteps();

			GoToStep(startStepIndex);
		}

        public void OnClickOnNextStep()
		{
			OnClickOnNextStep(false);
		}

		public void OnClickOnNextStep(bool forceAllow = false)
		{
			if (!forceAllow && !steps[currentStepIndex].GoToNextStepAllowed())
			{
				return;
			}

			if (IsLastStep && NextOnLastStepAction != null)
			{
				steps[currentStepIndex].OnExitStep();
				NextOnLastStepAction.Invoke();
				return;
			}

			GoToStep(currentStepIndex + 1);
		}

		public void OnClickOnPreviousStep()
		{
			OnClickOnPreviousStep(false);
		}

		public void OnClickOnPreviousStep(bool forceAllow = false)
		{
			if (!forceAllow && !steps[currentStepIndex].GoToPreviousStepAllowed())
			{
				return;
			}

			if (IsFirstStep && PreviousOnFirstStepAction != null)
			{
				steps[currentStepIndex].OnExitStep();
				PreviousOnFirstStepAction.Invoke();
				return;
			}

			GoToStep(currentStepIndex - 1);
		}

		public void GoToStep(int index)
		{
			// Desactivate the current step.
			if (currentStepIndex >= 0 && currentStepIndex < steps.Length)
			{
				steps[currentStepIndex].OnExitStep();

				GameObject currentStep = steps[currentStepIndex].gameObject;
				if (currentStep != null)
				{
					currentStep.SetActive(false);
				}
			}

			currentStepIndex = index;

			// Activate the new step.
			AbstractStep newStep = steps[currentStepIndex];
			newStep.gameObject.SetActive(true);
			newStep.OnEnterStep();
		}

		#endregion
	}
}