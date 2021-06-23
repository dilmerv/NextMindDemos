using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ImageFadeTransition : TweeningTransition
{
	[Header("Slide Properties")]
	[SerializeField] 
	private Color initialColor = default;
	[SerializeField] 
	private Color finalColor = default;

	private Image target = null;

	private void Awake()
	{
		target = GetComponent<Image>();

		finalColor = target.color;
		target.color = initialColor;
	}

	protected override void UpdateAnimation(float progress)
	{
		target.color = Color.Lerp(initialColor, finalColor, Mathf.Min(progress * speed, 1f));
	}
}