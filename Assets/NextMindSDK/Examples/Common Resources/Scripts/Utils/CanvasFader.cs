using System.Collections;
using UnityEngine;

namespace NextMind.Examples.Utility
{
	/// <summary>
	/// Helper component used to fade in or out a CanvasGroup.
	/// </summary>
	[RequireComponent(typeof(CanvasGroup))]
	public class CanvasFader : MonoBehaviour
	{
		[SerializeField]
		private bool initialStateIsVisible = false;

		[SerializeField]
		private bool fadeOnEnabled = true;

		[SerializeField]
		private float fadeInDuration = 0.5f;

		[SerializeField]
		private float fadeOutDuration = 0.5f;

		[SerializeField]
		private bool disableOnFadeOut = false;

		private CanvasGroup canvasGroup;

		/// <summary>
		/// Is this canvas currenlty enabled and visible?
		/// </summary>
		public bool IsCanvasVisible => this.gameObject.activeInHierarchy && Mathf.Approximately(canvasGroup.alpha, 1);

		private void Awake()
		{
			canvasGroup = GetComponent<CanvasGroup>();
			canvasGroup.alpha = initialStateIsVisible ? 1 : 0;
		}

		void OnEnable()
		{
			if (fadeOnEnabled)
			{
				StartFade(true);
			}
		}

		public void StartFade(bool fadeIn)
		{
			StartFade(fadeIn, false);
		}

		public void StartFade(bool fadeIn, bool immediate)
		{
			int targetAlpha = fadeIn ? 1 : 0;

			if (immediate)
			{
				canvasGroup.alpha = targetAlpha;
			}
			else
			{
				StartCoroutine(Fade(targetAlpha, fadeIn ? fadeInDuration : fadeOutDuration));
			}
		}

		public IEnumerator Fade(int targetAlpha, float fadeDuration = -1)
		{
			float elapsedTime = 0;
			Vector2 rangeFade = new Vector2(canvasGroup.alpha, targetAlpha);

			if (Mathf.Approximately(fadeDuration, -1))
			{
				fadeDuration = targetAlpha > canvasGroup.alpha ? fadeInDuration : fadeOutDuration;
			}

			while (elapsedTime < fadeDuration)
			{
				canvasGroup.alpha = Mathf.Lerp(rangeFade.x, rangeFade.y, elapsedTime / fadeDuration);

				elapsedTime += Time.deltaTime;
				yield return null;
			}

			canvasGroup.alpha = rangeFade.y;

			if (targetAlpha == 0 && disableOnFadeOut)
			{
				this.gameObject.SetActive(false);
			}
		}
	}
}