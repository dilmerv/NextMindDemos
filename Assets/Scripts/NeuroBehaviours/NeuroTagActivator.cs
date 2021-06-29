using UnityEngine;
using UnityEngine.Events;

public class NeuroTagActivator : NeuroTagSimple
{
    [SerializeField]
    private float minConfidence = 0.15f;

    public UnityEvent OnTriggerOn;

    public override void OnTriggered()
    {
        base.OnTriggered();
        OnTriggerOn?.Invoke();
    }
}
