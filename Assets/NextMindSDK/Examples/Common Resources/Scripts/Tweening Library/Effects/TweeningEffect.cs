using UnityEngine;

public abstract class TweeningEffect : MonoBehaviour
{
	public EnumAnimationGroup animationGroup;

	public abstract void Toggle(float multiplier);
}
