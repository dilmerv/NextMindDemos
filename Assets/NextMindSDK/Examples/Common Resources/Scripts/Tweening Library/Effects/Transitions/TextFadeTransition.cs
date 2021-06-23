using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class TextFadeTransition : TweeningTransition
{
	[Header("Fade Properties")]
	[SerializeField] 
	private Color initialColor = default;
	[SerializeField] 
	private Color finalColor = default;

	private Text target = null;

	private void Awake()
	{
		target = GetComponent<Text>();
		finalColor = target.color;

		target.color = initialColor;
	}

	protected override void UpdateAnimation(float progress)
	{
		target.color = Color.Lerp(initialColor, finalColor, Mathf.Min(progress, 1f));
	}

}
