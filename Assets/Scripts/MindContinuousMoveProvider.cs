using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class MindContinuousMoveProvider : ContinuousMoveProviderBase
{
    [SerializeField]
    //TODO this will be renamed to forwardMindControl
    //I also need a couple of more for multiple directions
    private NeuroTagSimple mindControl;

    protected override Vector2 ReadInput()
    {
        return new Vector2(0, mindControl.LastConfidenceValue);
    }
}
