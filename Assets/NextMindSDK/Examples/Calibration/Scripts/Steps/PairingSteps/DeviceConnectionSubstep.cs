using NextMind.Devices;
using NextMind.Examples.Steps;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NextMind.Examples.Calibration
{
    /// <summary>
    /// This substep and the following are managing this pairing feature like this :
    /// 1. Show a first dialogue box display Paired/Connected/Connecting devices AND  a "Pair new device" button. The user can else select the device and go directly to the next part, or pair a new device.
    /// 2. If the user clicked on "Pair new device", a new dialogue box is shown, asking the user to switch the device in pairing mode. 
    /// 3. A new dialogue box displays the unpaired available devices. The user must click on the device he wants to pair.
    /// 3. When the pairing has been done, another dialogue box prompts the player to confirm the pairing.
    /// 4. The last panel just display that all went well.
    /// </summary>
    public class DeviceConnectionSubstep : AbstractStep
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
        /// <summary>
        /// The prefab of the UI representation of a Device.
        /// </summary>
        [SerializeField]
        private DeviceUIElement deviceUiElementPrefab = null;

        /// <summary>
        /// The parent step.
        /// </summary>
        [SerializeField]
        private DeviceConnectionStep deviceConnectionStep = null;

        /// <summary>
        /// The text element used to display a message when no device is available.
        /// </summary>
        [SerializeField]
        private Text disclaimer = null;

        [SerializeField]
        protected Button nextButton = null;

        private Dictionary<uint, DeviceUIElement> devicesByID;

        #region AbstractStep implementation

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
        }

        public override bool GoToNextStepAllowed()
        {
            // We allow to go to the next step only if a device is connected.
            return devicesByID.Count > 0;
        }

        #endregion

        public void OnClickOnPairDevice()
        {
            deviceConnectionStep.OnStartPairingSteps();
            stepsManager.OnClickOnNextStep(true);
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
            if (device.ConnectionStatus == ConnectionStatus.Connecting || device.IsConnected || device.Paired)
            {
                if (!devicesByID.ContainsKey(device.ID))
                {
                    DeviceUIElement uiDevice = Instantiate(deviceUiElementPrefab);

                    uiDevice.transform.SetParent(availableDevicesListContainer, false);

                    devicesByID.Add(device.ID, uiDevice);
                }

                // Init the ui element with conncted device values. If the user click on this device when it is already connected, go to the next step. 
                devicesByID[device.ID].Init(device, () => { nextButton.onClick.Invoke(); });
            }
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
            disclaimer.text = "Select your device";

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

            if (NeuroManager.Instance.Devices.Count <= 0)
            {
                disclaimer.text = "No device available.";
            }
        }

        private void OnDeviceConnected(Device device)
        {
            nextButton.interactable = true;
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