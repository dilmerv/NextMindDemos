# How to use the example scenes

To run the examples, please ensure first that the 3 scenes ("Hub", "Calibration", "SDKDiscovery") are properly referenced in the project's Build Settings.
Then run the "Hub" scene. "Calibration" and "SDKDiscovery" scenes will be loaded automatically from the Hub scene by clicking on the dedicated buttons.

# VR scenes usage

To run the VR scenes, your project has to be configured to do so. Thus, you have to import the following packages from the Package Manager:

- "XR Plugin Management"
- "XR Interaction Toolkit"
- The specific package for the device you intend to use. (e.g. "Oculus XR Plugin" if you intend to use an Oculus headset).

Please visit Unity's official XR documentation (https://docs.unity3d.com/Manual/configuring-project-for-xr.html) for further details.