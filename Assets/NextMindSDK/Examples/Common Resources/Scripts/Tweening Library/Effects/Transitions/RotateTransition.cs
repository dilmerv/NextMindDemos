using UnityEngine;

public class RotateTransition : TweeningTransition
{
	[Header("Slide Properties")]
	[SerializeField] 
	private Vector3 initialEulerRotation = default;
	[SerializeField] 
	private Vector3 finalEulerRotation = default;

	Quaternion initialQuaternionRotation = default;
	Quaternion finalQuaternionRotation = default;

	private void Awake()
	{
		finalQuaternionRotation = transform.localRotation;
		finalEulerRotation = finalQuaternionRotation.eulerAngles;

		initialQuaternionRotation.eulerAngles = initialEulerRotation;

		transform.localRotation = initialQuaternionRotation;
	}

	protected override void UpdateAnimation(float progress)
	{
		transform.localRotation = Quaternion.Lerp(initialQuaternionRotation, finalQuaternionRotation , Mathf.Min(progress, 1f));
	}
}
