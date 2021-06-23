using UnityEngine;
using UnityEngine.UI;

namespace NextMind.Devices
{
    /// <summary>
    /// This component displays basic information about the last connected <see cref="Device"/>.
    /// </summary>
    public class ConnectedDeviceStatus : MonoBehaviour
    {
        /// <summary>
        /// Image representing the battery level.
        /// </summary>
        [SerializeField]
        private Image batteryBar = null;

        /// <summary>
        /// Text displaying the battery level in percentage.
        /// </summary>
        [SerializeField]
        private Text batteryText = null;

        /// <summary>
        /// Text displaying the device's name.
        /// </summary>
        [SerializeField]
        private Text deviceName = null;

        /// <summary>
        /// The device from which we get the information to display.
        /// </summary>
        private Device connectedDevice = null;

        #region Unity Methods

        private void Start()
        {
            Initialize();
        }

        private void Update()
        {
            UpdateUI();
        }

        private void OnDestroy()
        {
            NeuroManager neuroManager = NeuroManager.Instance;
            if (neuroManager)
            {
                neuroManager.onDeviceConnected.RemoveListener(OnDeviceConnected);
                neuroManager.onDeviceDisconnected.RemoveListener(OnDeviceDisconnected);
            }
        }

        #endregion

        /// <summary>
        /// Get the connected device if it exists and start listening to the connections/disconnections.
        /// </summary>
        private void Initialize()
        {
            NeuroManager neuroManager = NeuroManager.Instance;
            if (neuroManager.ConnectedDevices.Count > 0)
            {
                OnDeviceConnected(neuroManager.ConnectedDevices[0]);
            }

            neuroManager.onDeviceConnected.AddListener(OnDeviceConnected);
            neuroManager.onDeviceDisconnected.AddListener(OnDeviceDisconnected);
        }

        /// <summary>
        /// Update each field of the UI
        /// </summary>
        private void UpdateUI()
        {
            if (connectedDevice != null)
            {
                deviceName.text = connectedDevice.Name;

                uint level = connectedDevice.GetBatteryLevel();
                batteryText.text = level + "%";
                batteryBar.fillAmount = level / 100;
            }
            else
            {
                deviceName.text = "--";
                batteryText.text = "--";
                batteryBar.fillAmount = 0;
            }
        }

        #region Connection events 

        private void OnDeviceConnected(Device device)
        {
            connectedDevice = device;
        }

        private void OnDeviceDisconnected(Device device)
        {
            if (connectedDevice != null && connectedDevice == device)
            {
                connectedDevice = null;
            }
        }

        #endregion
    }
}