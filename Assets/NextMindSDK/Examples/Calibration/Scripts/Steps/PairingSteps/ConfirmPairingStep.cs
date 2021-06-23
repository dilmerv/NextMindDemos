using NextMind.Devices;
using NextMind.Examples.Steps;

namespace NextMind.Examples.Calibration
{ 
    /// <summary>
    /// During this substep, the user is prompted to confirm the pairing.
    /// </summary>
    public class ConfirmPairingStep : AbstractStep
    {
        public override void OnEnterStep()
        {
            // We know that the user confirmed pairing when the device is connected.
            NeuroManager.Instance.onDeviceConnected.AddListener(OnDeviceConnected);
        }

        public override void OnExitStep()
        {
            NeuroManager.Instance.onDeviceConnected.RemoveListener(OnDeviceConnected);
        }

        private void OnDeviceConnected(Device device)
        {
            // Device is connected, switch to the next step.
            stepsManager.OnClickOnNextStep();
        }
    }
}