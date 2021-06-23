using UnityEngine;

public class SlideTransition : TweeningTransition
{
	[Header("Slide Properties")]
	[SerializeField]
	private Vector3 initialPosition = default;
	[SerializeField]
	private Vector3 finalPosition = default;

	RectTransform rectTransform = default;

	private void Awake()
	{
		if (rectTransform = GetComponent<RectTransform>())
		{
			finalPosition = rectTransform.anchoredPosition;
			rectTransform.anchoredPosition = initialPosition;
		}
		else
		{
			finalPosition = transform.localPosition;
			transform.localPosition = initialPosition;
		}
	}

	protected override void UpdateAnimation(float progress)
	{
		Vector3 nextPosition = Vector3.Lerp(initialPosition, finalPosition, Mathf.Min(progress, 1f));

		if (rectTransform)
		{
			rectTransform.anchoredPosition = nextPosition;
		}
		else
		{
			transform.localPosition = nextPosition;
		}
	}
}
