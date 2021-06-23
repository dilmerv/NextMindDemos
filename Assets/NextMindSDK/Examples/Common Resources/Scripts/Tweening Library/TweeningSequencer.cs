using System.Collections.Generic;
using UnityEngine;

public class TweeningSequencer : MonoBehaviour
{
	[Header("Animations")]
	[SerializeField]
	private List<TweeningEffect> effects = null;


	[Header("Properties")]
	[SerializeField]
	private float speedMultiplier = 1f;

	public int CurrentTransition { get; private set; } = 0;

	public bool IsLastTransition => CurrentTransition == effects.Count - 1;
	public bool IsFirstTransition => CurrentTransition == 0;

	[Header("AutoPlay")]
	[SerializeField]
	private bool isAutoPlayEnabled = true;
	[SerializeField]
	private float transitionDelay = 3f;
	private float timer = 0f;


	private void Awake()
	{
		effects[CurrentTransition].Toggle(speedMultiplier);
	}

	private void Update()
	{
		if (isAutoPlayEnabled)
		{
			timer += Time.deltaTime;

			if(timer >= transitionDelay)
			{
				NextTransition();
			}
		}
	}


	public void NextTransition()
	{
		GoToTransition((CurrentTransition + 1) % effects.Count);
	}

	public void PreviousTransition()
	{
		GoToTransition((CurrentTransition + (effects.Count - 1)) % effects.Count);
	}

	public void GoToTransition(int index)
	{
		timer = 0f;
		CloseTransition(CurrentTransition);
		CurrentTransition = index;
		OpenTransition(CurrentTransition);
	}

	private void CloseTransition(int index)
	{
		effects[index].Toggle(-speedMultiplier);
	}

	private void OpenTransition(int index)
	{
		effects[index].Toggle(speedMultiplier);
	}
}
