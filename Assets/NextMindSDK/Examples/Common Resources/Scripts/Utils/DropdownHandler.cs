using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace NextMind.Examples.Utility
{
    public class DropdownHandler : MonoBehaviour
    {
        [SerializeField]
        private List<Button> items;

        [Header("Sensor Status Overlay")]
        /// <summary>
        /// Bool, true if Sensor status overlay is active, false otherwise.
        /// </summary>
        private bool isSensorStatusDisplayed = true;

        /// <summary>
        /// Section containing all information relative the the currently connected device.
        /// </summary>
        [SerializeField]
        private GameObject sensorStatusDisplay = null;

        /// <summary>
        /// Activates/Deactivates the overlay when menu is closed.
        /// </summary>
        public void ToggleSensorStatusDisplay()
        {
            isSensorStatusDisplayed = !isSensorStatusDisplayed;

            if (isSensorStatusDisplayed)
            {
                sensorStatusDisplay.SetActive(true);
            }
            else
            {
                sensorStatusDisplay.SetActive(false);
            }
        }

        public void GoToHub()
        {
            HubManager.Instance.BackToHubScene();
        }

        /// <summary>
        /// Exit the Application.
        /// </summary>
        public void Exit()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }
    }
}