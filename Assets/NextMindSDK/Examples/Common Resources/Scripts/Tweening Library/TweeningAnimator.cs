using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum EnumAnimationGroup 
{ 
	Dropdown,
	BurgerHighlight,
}

public class TweeningAnimator : MonoBehaviour
{
	[SerializeField] 
	private EnumAnimationGroup animationGroup = default;
	[SerializeField]
	private bool closeOnStartup = true;

	[Space]

	[SerializeField] 
	private List<GameObject> animatedObject = null;

	[SerializeField][Range(0f, 10f)] 
	private float speedMultiplier = 1f;


	private List<TweeningEffect> tweenEffects = new List<TweeningEffect>();

	private void Start()
	{
		animatedObject.ForEach(x => tweenEffects.AddRange(x.GetComponents<TweeningEffect>().Where(y => y.animationGroup == this.animationGroup)));

		if (closeOnStartup)
		{
			TogglePlay();
		}
	}

	public void TogglePlay()
	{
		speedMultiplier = -speedMultiplier;
		tweenEffects.ForEach(x => x.Toggle(speedMultiplier));
	}
}
