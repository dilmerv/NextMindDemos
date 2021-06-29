using NextMind.NeuroTags;
using UnityEngine;

[RequireComponent(typeof(NeuroTag))]
public class NeuroTagColorRange : NeuroTagSimple
{
    [SerializeField]
    private float minConfidence = 0.5f;

    [SerializeField]
    private Color targetColor = Color.red;

    private Color initialColor;

    private Renderer neuroTagRenderer;

    public override void OnSetup()
    {
        base.OnSetup();

        neuroTagRenderer = visualObject.GetComponent<Renderer>();
        initialColor = neuroTagRenderer.material.color;
    }
    
    public override void OnConfidenceChanged(float value)
    {
        base.OnConfidenceChanged(value);

        if (value >= minConfidence)
        {
            neuroTagRenderer.material.color = targetColor;
        }
        else
        {
            neuroTagRenderer.material.color = initialColor;
        }
    }
}
