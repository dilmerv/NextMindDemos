using UnityEngine;

namespace NextMind.Utility
{
    /// <summary>
    /// This component keeps the physical scale of the object at the same value it has 
    /// when seen by the main camera from 1 meter distance (if OneMeter is set as referenceDistance. Else the start distance from main camera to the object is used).
    /// </summary>
    public class ConstantPhysicalSize : MonoBehaviour
    {
        [SerializeField]
        private bool hasMaxSize = false;
        [SerializeField]
        private Vector3 maxSize = Vector3.zero;

        private Vector3 originalScale;
        private float originalDistance;

        enum ReferenceDistance
        {
            OneMeter,
            StartDistance,
        }
        [SerializeField]
        private ReferenceDistance referenceDistance = ReferenceDistance.OneMeter;

        private void Awake()
        {
            originalScale = this.transform.localScale;
            if (referenceDistance == ReferenceDistance.StartDistance)
            {
                originalDistance = Vector3.Distance(Camera.main.transform.position, this.transform.position);
            }
        }

        void Update()
        {
            Vector3 newScale = originalScale * Vector3.Distance(Camera.main.transform.position, this.transform.position);
            if (referenceDistance == ReferenceDistance.StartDistance)
            {
                newScale /= originalDistance;
            }

            if (hasMaxSize)
            {
                newScale.x = Mathf.Min(newScale.x, maxSize.x);
                newScale.y = Mathf.Min(newScale.y, maxSize.y);
                newScale.z = Mathf.Min(newScale.z, maxSize.z);
            }

            this.transform.localScale = newScale;
        }
    }
}