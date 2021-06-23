using NextMind.Examples.Steps;
using NextMind.Devices;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NextMind.Examples.Calibration
{
    /// <summary>
    /// Implementation of an <see cref="AbstractStep"/> managed by the <see cref="StepsManager"/>.
    /// During this step, the user has to select the device he wants to connect to.
    /// </summary>
    public class SelectDeviceStep : AbstractStep
    {
        /// <summary>
        /// Moving part of the scanning feedback.
        /// </summary>
        [SerializeField]
        private Image loadingBar = null;
        /// <summary>
        /// Fixed part of the scanning feedback.
        /// </summary>
        [SerializeField]
        private Image loadingBackground = null;

        /// <summary>
        /// The RectTransform where to instantiate the devices' ui elements.
        /// </summary>
        [SerializeField]
        private RectTransform availableDevicesListContainer = null;
        [SerializeField]
        private DeviceUIElement deviceUiElementPrefab = null;

        /// <summary>
        /// The text element used to display a message when no device is available.
        /// </summary>
        [SerializeField]
        private Text disclaimer = null;

        [SerializeField]
        protected Button nextButton = null;

        private Dictionary<uint, DeviceUIElement> devicesByID;

        public override void OnEnterStep()
        {
            devicesByID = new Dictionary<uint, DeviceUIElement>();

            InitList();

            NeuroManager neuroManager = NeuroManager.Instance;

            nextButton.interactable = neuroManager.ConnectedDevices.Count > 0;

            ShowLoading(neuroManager.IsScanning);

            // Register to the connection events
            neuroManager.onDeviceAvailable.AddListener(OnDeviceAvailable);
            neuroManager.onDeviceUnavailable.AddListener(OnDeviceUnavailable);

            neuroManager.onScanningStarted.AddListener(OnScanningStarted);
            neuroManager.onScanningStopped.AddListener(OnScanningStopped);

            neuroManager.onDeviceConnected.AddListener(OnDeviceConnected);
            neuroManager.onDeviceDisconnected.AddListener(OnDeviceDisconnected);
        }

        public override void OnExitStep()
        {
            Clean();

            // Unregister from the connection events
            NeuroManager neuroManager = NeuroManager.Instance;
            neuroManager.onDeviceAvailable.RemoveListener(OnDeviceAvailable);
            neuroManager.onDeviceUnavailable.RemoveListener(OnDeviceUnavailable);

            neuroManager.onScanningStarted.RemoveListener(OnScanningStarted);
            neuroManager.onScanningStopped.RemoveListener(OnScanningStopped);

            neuroManager.onDeviceConnected.RemoveListener(OnDeviceConnected);
            neuroManager.onDeviceDisconnected.RemoveListener(OnDeviceDisconnected);
        }

        public override bool GoToNextStepAllowed()
        {
            // We allow to go to the next step only if a device is connected.
            return devicesByID.Count > 0;
        }

        private void InitList()
        {
            var devices = NeuroManager.Instance.Devices;

            if (devices.Count <= 0)
            {
                disclaimer.text = "No device available.";
                return;
            }

            foreach (var device in devices)
            {
                AddDevice(device);
            }
        }

        /// <summary>
        /// Instantiate a device UI element in the list.
        /// </summary>
        /// <param name="device">The Device which has been connected</param>
        private void AddDevice(Device device)
        {
            if (!devicesByID.ContainsKey(device.ID))
            {
                DeviceUIElement uiDevice = Instantiate(deviceUiElementPrefab);

                uiDevice.transform.SetParent(availableDevicesListContainer, false);

                devicesByID.Add(device.ID, uiDevice);
            }

            // Init the ui element with conncted device values. If the user click on this device when it is already connected, go to the next step. 
            devicesByID[device.ID].Init(device, ()=> { nextButton.onClick.Invoke(); });
        }

        private void ShowLoading(bool show)
        {
            Color green = loadingBar.color;
            if (show)
            {
                green.a = 0.1f;
            }
            loadingBackground.color = green;

            loadingBar.gameObject.SetActive(show);
        }

        private void Clean()
        {
            foreach (var element in devicesByID)
            {
                Destroy(element.Value.gameObject);
            }
            devicesByID.Clear();
        }

        #region Connection events 

        /// <summary>
        /// If a device becomes available, add it to the list.
        /// </summary>
        /// <param name="device">The device which became available</param>
        private void OnDeviceAvailable(Device device)
        {
            disclaimer.text = "Select your device :";

            if (!devicesByID.ContainsKey(device.ID))
            {
                AddDevice(device);
            }
        }

        /// <summary>
        /// If a device becomes unavailable, remove it from the list.
        /// </summary>
        /// <param name="device">The device which became unavailable</param>
        private void OnDeviceUnavailable(Device device)
        {
            if (!device.IsConnected)
            {
                Destroy(devicesByID[device.ID].gameObject);
                devicesByID.Remove(device.ID);
            }

            if(NeuroManager.Instance.Devices.Count <= 0)
            {
                disclaimer.text = "No device available.";
            }
        }

        private void OnDeviceConnected(Device device)
        {
            nextButton.interactable = true;
        }

        private void OnDeviceDisconnected(Device device)
        {
            if (NeuroManager.Instance.ConnectedDevices.Count == 0)
            { 
                nextButton.interactable = false;
            }
        }

        #endregion

        #region Scanning events

        private void OnScanningStarted()
        {
            ShowLoading(true);
        }

        private void OnScanningStopped()
        {
            ShowLoading(false);
        }

        #endregion
    }
}