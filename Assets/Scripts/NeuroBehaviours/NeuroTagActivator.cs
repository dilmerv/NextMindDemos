using NextMind.NeuroTags;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(NeuroTag))]
public class NeuroTagActivator : NeuroTagSimple
{
    [SerializeField]
    private float minConfidence = 0.1f;

    [SerializeField]
    private UnityEvent OnTriggerOn;

    private bool isActivated = false;

    public override void OnConfidenceChanged(float value)
    {
        base.OnConfidenceChanged(value);

        if (isActivated)
            return;

        if (value >= minConfidence)
        {
            OnTriggerOn?.Invoke();
            isActivated = true;
        }
    }
}
