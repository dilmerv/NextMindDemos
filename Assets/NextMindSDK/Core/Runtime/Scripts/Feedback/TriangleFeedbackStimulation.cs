using NextMind.NeuroTags;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This component makes the triangle feedback react to stimulation, by switching between <see cref="defaultColor"/> to <see cref="stimulatedColor"/>.
/// </summary>
public class TriangleFeedbackStimulation : MonoBehaviour
{
    /// <summary>
    /// The NeuroTag from which we get the stimulation values.
    /// </summary>
    [SerializeField]
    private NeuroTag neuroTag;
    
    /// <summary>
    /// The material instantiated on the triangle.
    /// </summary>
    [SerializeField]
    private Material material = null;

    /// <summary>
    /// The color applied in case of a stimulation value lower than 0.5.
    /// </summary>
    [SerializeField]
    private Color defaultColor = default;
    /// <summary>
    /// The color applied in case of a stimulation value greater than 0.5.
    /// </summary>
    [SerializeField]
    private Color stimulatedColor = default;

    /// <summary>
    /// Should we hide the triangle feedback when the linked NeuroTag becoms inactive?
    /// </summary>
    [SerializeField]
    private bool autoHideFeedback = default;

    #region Unity methods

    private void Awake()
    {
        if (neuroTag == null)
        {
            // Find the component in parents.
            neuroTag = GetComponentInParent<NeuroTag>();
        }

        if (neuroTag != null)
        {
            neuroTag.onStimulationStateUpdated.AddListener(OnStimulationStateUpdated);

            neuroTag.onBecameActivated.AddListener(OnNeuroTagActivationChanged);
            neuroTag.onBecameDeactivated.AddListener(OnNeuroTagActivationChanged);

            // Set initial state.
            OnNeuroTagActivationChanged();
        }

        // Force to create an instance of the given material.
        material = new Material(material);
        SetMaterials();
    }

    private void OnDestroy()
    {
        if (neuroTag != null)
        {
            neuroTag.onStimulationStateUpdated.RemoveListener(OnStimulationStateUpdated);

            neuroTag.onBecameActivated.RemoveListener(OnNeuroTagActivationChanged);
            neuroTag.onBecameDeactivated.RemoveListener(OnNeuroTagActivationChanged);
        }
    }

    #endregion

    #region Event callbacks

    /// <summary>
    /// Apply the right color regarding the stimulation value.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="value"></param>
    public void OnStimulationStateUpdated(GameObject target, float value)
    {
        material.color = (value > 0.5f) ? stimulatedColor : defaultColor;
    }

    /// <summary>
    /// Show the feedback when the NeuroTag becomes active, hide it otherwise.
    /// </summary>
    private void OnNeuroTagActivationChanged()
    {
        if (autoHideFeedback)
        {
            SetVisible(neuroTag.IsVisible);
        }
    }

    #endregion

    /// <summary>
    /// Set the material on all the children.
    /// </summary>
    private void SetMaterials()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform t = transform.GetChild(i);
            Renderer r = t.GetComponent<Renderer>();
            if (r != null)
            {
                r.sharedMaterial = material;
            }
            else
            {
                Image image = t.GetComponent<Image>();
                if (image != null)
                {
                    image.material = material;
                }
            }
        }
    }

    /// <summary>
    /// Show/Hide the sides of the triangle.
    /// </summary>
    /// <param name="visible"></param>
    private void SetVisible(bool visible)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(visible);
        }
    }
}
