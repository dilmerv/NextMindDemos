using DilmerGames.Core.Singletons;
using NextMind;
using NextMind.Devices;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(NeuroManager))]
public class DeviceManager : Singleton<DeviceManager>
{
    [SerializeField]
    private TextMeshProUGUI deviceStatus;

    private NeuroManager neuroManager;

    private Device connectedDevice;

    private void Awake()
    {
        neuroManager = GetComponent<NeuroManager>();

        neuroManager.onDeviceAvailable.AddListener(OnDeviceStatusChanged);
        neuroManager.onDeviceDisconnected.AddListener(OnDeviceStatusChanged);
        neuroManager.onDeviceAvailable.AddListener(OnDeviceStatusChanged);
        neuroManager.onDeviceDisconnected.AddListener(OnDeviceStatusChanged);
        neuroManager.onDeviceUnavailable.AddListener(OnDeviceStatusChanged);

        connectedDevice.onConnectionStatusChanged.AddListener(OnDeviceConnectionChanged);
    }

    private void OnDeviceStatusChanged(Device device)
    {
        deviceStatus.text = $"{device.ConnectionStatus}";
    }

    private void OnDeviceConnectionChanged(ConnectionStatus status)
    {
        deviceStatus.text = $"{status}";
    }
}
