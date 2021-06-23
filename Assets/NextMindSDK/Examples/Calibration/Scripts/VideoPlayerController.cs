using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoPlayerController : MonoBehaviour
{
    [SerializeField]
    private VideoPlayer videoPlayer;

    [SerializeField]
    private Image pauseIcon;
    [SerializeField]
    private Image playIcon;

    public VideoPlayer.EventHandler LoopPointReached;

    #region Unity methods

    private void Start()
    {
        InitProgressBar();

        videoPlayer.loopPointReached += VideoPlayer_loopPointReached;
    }

    private void Update()
    {
        // Don't update the step if the help panel is displayed
        if (videoPlayer.isPlaying)
        {
            progressBarImage.fillAmount = (float)videoPlayer.frame / (float)videoPlayer.frameCount;
        }
    }

    private void FixedUpdate()
    {
        if (isHoveringProgressBar)
        {
            OnPointerHover_ProgressBar();
        }
    }

    private void OnEnable()
    {
        videoPlayer.Play();
    }

    private void OnDisable()
    {
        videoPlayer.Stop();
        videoPlayer.targetTexture.Release();
    }

    #endregion

    private Coroutine fadeOutCoroutine;
    private IEnumerator ShowAndFadeIcon(Image image)
    {
        float duration = 0.75f, currentTimer = 0f;
        float t = 0;
        Color startColor = image.color;
        startColor.a = 1;
        Color targetColor = new Color(startColor.r,startColor.g, startColor.b,0);

        image.color = startColor;

        while (t < 1)
        {
            image.color = Color.Lerp(startColor,targetColor,t);

            currentTimer += Time.deltaTime;
            t = currentTimer / duration;

            yield return null;
        }

        image.color = targetColor;
        fadeOutCoroutine = null;
    }

    private void VideoPlayer_loopPointReached(VideoPlayer source)
    {
        LoopPointReached?.Invoke(source);
    }

    #region Pointer management

    public void OnPointerClick_Video()
    {
        if (videoPlayer.isPlaying)
        {
            videoPlayer.Pause();
            if (fadeOutCoroutine != null)
            {
                Color transparent = playIcon.color;
                transparent.a = 0;
                playIcon.color = transparent;

                StopCoroutine(fadeOutCoroutine);
            }
            fadeOutCoroutine = StartCoroutine(ShowAndFadeIcon(pauseIcon));
        }
        else
        {
            videoPlayer.Play();

            if (fadeOutCoroutine != null)
            {
                Color transparent = pauseIcon.color;
                transparent.a = 0;
                pauseIcon.color = transparent;

                StopCoroutine(fadeOutCoroutine);
            }
            fadeOutCoroutine = StartCoroutine(ShowAndFadeIcon(playIcon));
        }
    }

    #endregion 

    #region Progress bar management

    [SerializeField]
    private RectTransform progressBar;

    private Image progressBarImage;

    [SerializeField]
    private RectTransform cursor;

    private Vector2 initialSizeDelta;

    private bool isHoveringProgressBar => cursor.gameObject.activeSelf;

    private void InitProgressBar()
    {
        initialSizeDelta = progressBar.sizeDelta;
        progressBarImage = progressBar.GetComponent<Image>();
    }

    public void OnPointerEnter_ProgressBar()
    {
        progressBar.sizeDelta = new Vector2(initialSizeDelta.x, initialSizeDelta.y * 2);

        cursor.gameObject.SetActive(true);
    }

    public void OnPointerHover_ProgressBar()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(progressBar, Input.mousePosition, Camera.main, out Vector2 localPoint);

        cursor.anchoredPosition = new Vector2(localPoint.x, 0);
    }

    public void OnPointerExit_ProgressBar()
    {
        progressBar.sizeDelta = initialSizeDelta;

        cursor.gameObject.SetActive(false);
    }

    public void OnPointerClick_ProgressBar()
    {
        if (isHoveringProgressBar)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(progressBar, Input.mousePosition, Camera.main, out Vector2 localPoint);

            float t = (localPoint.x + (progressBar.pivot.x * progressBar.rect.width )) / progressBar.rect.width;
            
            videoPlayer.frame = Mathf.RoundToInt(t * videoPlayer.frameCount);

        }
    }

    #endregion
}
