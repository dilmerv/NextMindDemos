using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using NextMind.Core.Event;

namespace NextMind.Examples.Utility
{
    /// <summary>
    /// The EventNotifier is a component listening to all events (about devices connection, bluetooth, etc...).
    /// It displays message on a Text elements, colored regarding the event severity (Status, Warning, Error).
    /// </summary>
    public class EventNotifier : MonoBehaviour
    {
        /// <summary>
        /// The canvas group on which text is displayed. Used to show/hide elements using its alpha value.
        /// </summary>
        [SerializeField]
        private CanvasGroup canvasGroup = null;

        /// <summary>
        /// The text element.
        /// </summary>
        [SerializeField]
        private Text messageText = null;

        /// <summary>
        /// The button used to hide the displayed message.
        /// </summary>
        [SerializeField]
        private Image closeButtonImage = null;

        /// <summary>
        /// The background which will change color following events Severity as well.
        /// </summary>
        [SerializeField]
        private Image background = null;

        /// <summary>
        /// The coroutine used to fade in/out the canvasGroup.
        /// </summary>
        private Coroutine showCoroutine;

        private string currentDisplayedMessage => messageText.text;

        #region Unity methods

        private void Start()
        {
            // Start listening to wanted events.
            var neuroManager = NeuroManager.Instance;
            if (neuroManager != null)
            {
                neuroManager.onGlobalStatusEvent.AddListener(OnGlobalStatusEvent);
                neuroManager.onDeviceEvent.AddListener(OnDeviceEvent);
            }
        }

        private void OnDestroy()
        {
            var neuroManager = NeuroManager.Instance;
            if (neuroManager != null)
            {
                // Stop listening to wanted events.
                neuroManager.onGlobalStatusEvent.RemoveListener(OnGlobalStatusEvent);
                neuroManager.onDeviceEvent.RemoveListener(OnDeviceEvent);
            }
        }

        #endregion

        #region Events handling 

        /// <summary>
        /// Method triggered when receiving a GlobalStatusEvent.
        /// </summary>
        /// <param name="globalEvent">The event received</param>
        private void OnGlobalStatusEvent(EventBase globalEvent)
        {
            // If an identical message is currently displayed, don't do anything
            if (showCoroutine!=null && string.Compare(globalEvent.ToString(), currentDisplayedMessage) == 0)
            {
                return;
            }

            if (globalEvent is BCIEvent bciEvent)
            {
                DisplayMessage(bciEvent.ToString(), globalEvent.Severity);
            }
            else if (globalEvent is BLEEvent bleEvent)
            {
                DisplayMessage(bleEvent.ToString(), globalEvent.Severity);
            }
        }

        /// <summary>
        /// Method triggered when receiving a DeviceEvent.
        /// </summary>
        /// <param name="deviceID">The device's id</param>
        /// <param name="deviceEvent">The event received</param>
        private void OnDeviceEvent(int deviceID, EventBase deviceEvent)
        {
            // If an identical message is currently displayed, don't do anything
            if (showCoroutine != null && string.Compare(deviceEvent.ToString(), currentDisplayedMessage) == 0)
            {
                return;
            }

            if (deviceEvent is BCIEvent bciEvent)
            {
                DisplayMessage(bciEvent.ToString(), deviceEvent.Severity);
            }
            else if (deviceEvent is BLEEvent bleEvent)
            {
                DisplayMessage(bleEvent.ToString(), deviceEvent.Severity);
            }
        }

        #endregion

        /// <summary>
        /// Display the message applying the parameters values.
        /// </summary>
        /// <param name="text">The event's message</param>
        /// <param name="severity">The event's severity</param>
        private void DisplayMessage(string text, EventBase.EventSeverity severity)
        {
            messageText.text = text;
            switch (severity)
            {
                case EventBase.EventSeverity.Status:
                    // Blue
                    messageText.color = new Color32(0x00, 0x11, 0x7A, 0xFF);
                    background.color = new Color32(0xA8, 0xD0, 0xFF, 0xB2);
                    break;
                case EventBase.EventSeverity.Warning:
                    // Yellow
                    messageText.color = new Color32(0x72, 0x3F, 0x00, 0xFF);
                    background.color = new Color32(0xFF, 0xDF, 0x99, 0xB2);
                    break;
                case EventBase.EventSeverity.Error:
                    // Red
                    messageText.color = new Color32(0x7A, 0x00, 0x00, 0xFF);
                    background.color = new Color32(0xFF, 0x8F, 0x8F, 0xB2);
                    break;
            }

            // Apply the same color on the text and the button
            closeButtonImage.color = messageText.color;

            // Stop the coroutine if it is running.
            if (showCoroutine != null)
            {
                StopCoroutine(showCoroutine);
            }
            // Show the panel.
            showCoroutine = StartCoroutine(Show(true));
        }

        /// <summary>
        /// Method triggered when user click on close button.
        /// </summary>
        public void OnClickOnClose()
        {
            // Stop the coroutine if it is running.
            if (showCoroutine != null)
            {
                StopCoroutine(showCoroutine);
            }

            // Fade the panel out.
            showCoroutine = StartCoroutine(Show(false));
        }

        /// <summary>
        /// The coroutine fading in and out the message canvasGroup.
        /// </summary>
        /// <param name="show"></param>
        /// <returns></returns>
        private IEnumerator Show(bool show)
        {
            float t = 0, duration = 1f, timer = 0f;
            float startValue = canvasGroup.alpha;
            float targetValue = show ? 1 : 0;

            while (t < 1)
            {
                canvasGroup.alpha = Mathf.Lerp(startValue, targetValue, t);

                timer += Time.deltaTime;
                t = timer / duration;
                yield return null;
            }

            canvasGroup.alpha = targetValue;

            if (show)
            {
                // Wait X seconds then fade out.
                yield return new WaitForSeconds(15f);
                StartCoroutine(Show(false));
            }

            showCoroutine = null;
        }
    }
}