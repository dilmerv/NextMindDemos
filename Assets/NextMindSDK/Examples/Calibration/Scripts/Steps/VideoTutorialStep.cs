using UnityEngine;
using UnityEngine.Video;

namespace NextMind.Examples.Steps
{
    /// <summary>
    /// Implementation of an <see cref="AbstractStep"/> managed by the <see cref="StepsManager"/>.
    /// During this step, the user can watch a video explaining how to set up the device.
    /// </summary>
    public class VideoTutorialStep : AbstractStep
    {
        [SerializeField]
        private VideoPlayerController videoPlayerGroup = null;

        #region AbstractStep implementation

        public override void OnEnterStep()
        {
            videoPlayerGroup.LoopPointReached += OnLoopPointReached;
        }

        public override void OnExitStep()
        {
            videoPlayerGroup.LoopPointReached -= OnLoopPointReached;
        }

        #endregion

        private void OnLoopPointReached(VideoPlayer player)
        {
            OnClickOnCloseVideo();
        }

        public void OnClickOnThumbnail()
        {
            videoPlayerGroup.gameObject.SetActive(true);
        }
     

        public void OnClickOnCloseVideo()
        {
            videoPlayerGroup.gameObject.SetActive(false);
        }
    }
}