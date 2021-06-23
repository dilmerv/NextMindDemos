using UnityEngine;

public abstract class TweeningTransition : TweeningEffect
{
	[SerializeField]
	protected float speed = 1f;
	private float currentSpeed;

	private float progress = 0f;
	private bool isUpdating = false;

	private void Awake()
	{
		currentSpeed = speed;
	}

	private void Update()
	{
		if (isUpdating)
		{
			progress += currentSpeed * Time.deltaTime;

			if (progress >= 1f)
			{
				progress = 1f;
				isUpdating = false;
			}
			else if (progress <= 0f)
			{
				progress = 0f;
				isUpdating = false;
			}

			UpdateAnimation(progress);
		}
	}

	protected abstract void UpdateAnimation(float progress);

	public override void Toggle(float multiplier)
	{
		currentSpeed = speed * multiplier;
		isUpdating = true;

		//Debug.Log(gameObject.name + " display = " + (speed > 0));
	}

	public void Play(float multiplier)
	{
		currentSpeed = Mathf.Abs(speed * multiplier);
		isUpdating = true;
	}

	public void PlayReverse(float multiplier)
	{
		currentSpeed = -Mathf.Abs(speed * multiplier);
		isUpdating = true;
	}
}
