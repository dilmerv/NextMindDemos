using NextMind.NeuroTags;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class NeuroTagDoor : NeuroTagSimple
{
    [SerializeField]
    private float minConfidence = 0.15f;

    [SerializeField]
    private float closeDoorAfterSeconds = 2.0f;

    [SerializeField]
    private Vector3 targetLocation = Vector3.zero;

    private Vector3 initialLocation;

    private bool isClosed = true;

    private float openTime;

    private AudioSource audioSource;

    public override void OnSetup()
    {
        base.OnSetup();
        initialLocation = transform.localPosition;
        audioSource = GetComponent<AudioSource>();
    }

    public override void UpdateNeuroTag()
    {
        if(!isClosed && Time.time - openTime > closeDoorAfterSeconds)
        {
            transform.localPosition = initialLocation;
            isClosed = true;
            audioSource?.Play();
        }
    }

    public override void OnConfidenceChanged(float value)
    {
        base.OnConfidenceChanged(value);

        if (value < minConfidence || !isClosed)
            return;

        transform.localPosition = targetLocation;
        isClosed = false;
        openTime = Time.time;
        audioSource?.Play();
    }
}
