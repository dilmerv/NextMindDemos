using NextMind.Devices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NextMind.Examples.Steps
{
    /// <summary>
    /// The representation of a <see cref="Device"/> on the UI.
    /// </summary>
    public class DeviceUIElement : MonoBehaviour
    {
        [SerializeField]
        private Text nameText = null;

        [SerializeField]
        private Text stateText = null;

        [SerializeField]
        private Image bottomLine = null;

        [SerializeField]
        private Color selectedOn = Color.green;

        [SerializeField]
        private Color selectedOff = Color.gray;

        [SerializeField]
        private GameObject connectingBar = null;

        [SerializeField]
        private GameObject overlaySelectOn = null;

        private Button button;

        private Device currentDevice;

        private UnityAction onValidateAction;

        private void Awake()
        {
            button = GetComponent<Button>();
        }

        private void OnDestroy()
        {
            currentDevice.onConnectionStatusChanged.RemoveListener(SetVisualState);
        }

        /// <summary>
        /// Initialize this instance from <paramref name="device"/> data.
        /// </summary>
        /// <param name="device">The device to use</param>
        /// <param name="onValidate">The action triggered on user click, if the device is connected</param>
        public void Init(Device device, UnityAction onValidate)
        {
            currentDevice = device;
            onValidateAction = onValidate;

            SetVisualState(device.ConnectionStatus);

            currentDevice.onConnectionStatusChanged.AddListener(SetVisualState);

            UpdateLabels();
        }

        private void UpdateLabels()
        {
            nameText.text = currentDevice.Name;
        }

        public void OnClickOnDevice()
        {
            // The validate action is launched only when the device is connected (it will select the device to use during the session), or unpaired (it will start pairing to the device)
            if (currentDevice.IsConnected || !currentDevice.Paired )
            {
                onValidateAction.Invoke();
            }
            else
            {
                var neuroManager = NeuroManager.Instance;
                // Disconnect the connected device if it exists
                if (neuroManager.ConnectedDevices.Count >= 1)
                {
                    neuroManager.DisconnectDevice(neuroManager.ConnectedDevices[0]);
                }

                if (!neuroManager.ConnectDevice(currentDevice))
                {
                    Debug.LogWarning("Cannot connect to device...");
                }
            }
        }

        /// <summary>
        /// Set color and texts regarding the device's connection status.
        /// </summary>
        /// <param name="state"></param>
        private void SetVisualState(ConnectionStatus state)
        {
            overlaySelectOn.SetActive(state == ConnectionStatus.Connected);
            connectingBar.SetActive(state == ConnectionStatus.Connecting);
            bottomLine.color = state == ConnectionStatus.Connected ? selectedOn : selectedOff;
            button.interactable = state != ConnectionStatus.Connecting;

            switch (state)
            {
                case ConnectionStatus.Available:
                    stateText.text = "STATE : Available";
                    break;
                case ConnectionStatus.Connecting:
                    stateText.text = "STATE : Connecting...";
                    break;
                case ConnectionStatus.Connected:
                    stateText.text = "STATE : Connected";
                    break;
            }
        }
    }
}