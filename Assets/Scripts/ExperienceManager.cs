using UnityEngine;

public class ExperienceManager : MonoBehaviour
{
    public void LogConfidence(float value)
    {
        Logger.Instance.LogInfo($"Confidence Value: {value}");
    }
}
