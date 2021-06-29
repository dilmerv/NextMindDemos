using NextMind.NeuroTags;
using UnityEngine;

[RequireComponent(typeof(NeuroTag))]
public class NeuroTagScale : NeuroTagSimple
{
    [SerializeField]
    private float pushForSeconds = 3.0f;

    [SerializeField]
    private Vector3 targetScale;

    private Vector3 initialScale;

    private float initialTime;

    public override void OnSetup()
    {
        base.OnSetup();
        initialScale = visualObject.localScale;
    }

    public override void OnTriggered()
    {
        initialTime = Time.time;
    }

    public override void OnMaintained()
    {
        base.OnMaintained();

        if (Time.time - initialTime > pushForSeconds)
        {
            SetButtonScale(targetScale);
        }
    }

    public override void OnReleased()
    {
        base.OnReleased();

        SetButtonScale(initialScale);
    }

    private void SetButtonScale(Vector3 newScale)
    {
        visualObject.localScale = newScale;
    }
}
