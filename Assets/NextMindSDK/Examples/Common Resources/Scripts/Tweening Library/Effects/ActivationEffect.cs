using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActivationEffect : TweeningEffect
{
	[SerializeField]
	private List<Behaviour> components = null;

	public override void Toggle(float multiplier)
	{
		components.ForEach(x =>
		{
			if (x.GetType() == typeof(Selectable))
			{
				(x as Selectable).interactable = !(x as Selectable).interactable;
			}
			else
			{
				x.enabled = !x.enabled;
			}
		});
	}
}
