using NextMind.NeuroTags;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(NeuroTag))]
public class NeuroTagSimple : MonoBehaviour
{
    [SerializeField]
    protected TextMeshProUGUI overlayValue;

    protected Transform visualObject;

    public float LastConfidenceValue { get; private set; }

    private void OnEnable()
    {
        this.OnSetup();    
    }

    private void Update()
    {
        this.UpdateNeuroTag();
    }

    public virtual void OnSetup()
    {
        if(overlayValue == null)
        {
            Debug.LogError("There was an error finding overlay value");
        }

        visualObject = transform.Find("Visual");
    }

    public virtual void UpdateNeuroTag()
    {}

    public virtual void OnConfidenceChanged(float value)
    {
        LastConfidenceValue = value;

        if (overlayValue == null) return;
        overlayValue.text = $"{value}";
    }

    public virtual void OnTriggered()
    {
        if (overlayValue == null) return;
        overlayValue.text = $"OnTriggered executed";
    }

    public virtual void OnMaintained()
    {
        if (overlayValue == null) return;
        overlayValue.text = $"OnMaintained executed";
    }

    public virtual void OnReleased()
    {
        if (overlayValue == null) return;
        overlayValue.text = $"OnReleased executed";
    }
}
