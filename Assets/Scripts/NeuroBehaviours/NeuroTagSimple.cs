using NextMind.NeuroTags;
using TMPro;
using UnityEngine;


[RequireComponent(typeof(NeuroTag))]
public class NeuroTagSimple : MonoBehaviour
{
    [SerializeField]
    protected TextMeshProUGUI overlayValue;

    protected Transform visualObject;

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
        overlayValue.text = $"{value}";
    }

    public virtual void OnTriggered()
    {
        overlayValue.text = $"OnTriggered executed";
    }
    public virtual void OnMaintained()
    {
        overlayValue.text = $"OnMaintained executed";
    }

    public virtual void OnReleased()
    {
        overlayValue.text = $"OnReleased executed";
    }
}
