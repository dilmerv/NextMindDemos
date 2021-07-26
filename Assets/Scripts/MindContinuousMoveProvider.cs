using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class MindContinuousMoveProvider : ContinuousMoveProviderBase
{
    [SerializeField]
    private float minConfidence = 0.3f;

    [SerializeField]
    private NeuroTagSimple forwardDirectionMindControl;

    [SerializeField]
    private NeuroTagSimple backwardDirectionMindControl;

    [SerializeField]
    private NeuroTagSimple leftDirectionMindControl;

    [SerializeField]
    private NeuroTagSimple rightDirectionMindControl;

    protected override Vector2 ReadInput()
    {
        float forwardDirection = 0;
        float leftRightDirection = 0;

        if(forwardDirectionMindControl.LastConfidenceValue >= minConfidence) // forward
            forwardDirection = forwardDirectionMindControl.LastConfidenceValue;
        if (backwardDirectionMindControl.LastConfidenceValue >= minConfidence) // backward
            forwardDirection = backwardDirectionMindControl.LastConfidenceValue * -1;

        if (rightDirectionMindControl.LastConfidenceValue >= minConfidence) // right
            leftRightDirection = rightDirectionMindControl.LastConfidenceValue;
        if (leftDirectionMindControl.LastConfidenceValue >= minConfidence) // left
            leftRightDirection = leftDirectionMindControl.LastConfidenceValue * -1;

        return new Vector2(leftRightDirection, forwardDirection) * moveSpeed;
    }
}
