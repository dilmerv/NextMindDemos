using UnityEngine;

public class ScaleTransition : TweeningTransition
{
	[Header("Scale Properties")]
	[SerializeField] 
	private Vector3 initialScale = default;
	[SerializeField] 
	private Vector3 finalScale = default;

	private void Awake()
	{
		// Allows UI to be built as displayed in the editor
		finalScale = transform.localScale;
		transform.localScale = initialScale;
	}

	protected override void UpdateAnimation(float progress)
	{
		transform.localScale = Vector3.Lerp(initialScale, finalScale, Mathf.Min(progress, 1f));
	}
}
