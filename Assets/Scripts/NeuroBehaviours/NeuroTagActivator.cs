using UnityEngine;
using UnityEngine.Events;

public class NeuroTagActivator : NeuroTagSimple
{
    [SerializeField]
    private float minConfidence = 0.1f;

    [SerializeField]
    private UnityEvent OnTriggerOn;

    private bool isActivated = false;

    public bool IsActivated
    {
        get { return IsActivated; }
        set { isActivated = value; }
    }

    public override void OnConfidenceChanged(float value)
    {
        if (isActivated)
            return;

        base.OnConfidenceChanged(value);
        
        if (value >= minConfidence)
        {
            OnTriggerOn?.Invoke();
        }
    }
}
