using UnityEngine;

namespace NextMind.Examples.Utility
{
    /// <summary>
    /// This component will place children of this GameObject on a circle around it.
    /// </summary>
    public class CircleLayout : MonoBehaviour
    {
        /// <summary>
        /// The cicrle radius.
        /// </summary>
        [SerializeField]
        private float radius = 1;

        /// <summary>
        /// The angle offset in degrees.
        /// </summary>
        [SerializeField]
        private float startOffset = 0;

        /// <summary>
        /// Should we position the children around the Z axis?
        /// </summary>
        [SerializeField]
        private bool aroundZAxis = true;

        /// <summary>
        /// Should the children look at the circle's center?
        /// </summary>
        [SerializeField]
        private bool lookAtCenter = false;

        /// <summary>
        /// Position the the children in a clockwise order?
        /// </summary>
        [SerializeField]
        private bool clockwise = false;

        public void Start()
        {
            PlaceObjects();
        }

        private void PlaceObjects()
        {
            int childCount = transform.childCount;

            for (int i = 0; i < childCount; i++)
            {
                float degAngle = 360f * ((float)i / childCount) + startOffset;
                float radAngle = degAngle * Mathf.Deg2Rad;

                Transform t = transform.GetChild(i);
                float sinValue = radius * Mathf.Sin(radAngle);
                if (clockwise)
                {
                    sinValue *= -1;
                }

                t.localPosition = new Vector3(radius * Mathf.Cos(radAngle), aroundZAxis? t.localPosition.y:sinValue, aroundZAxis? sinValue : t.localPosition.z);
                if (lookAtCenter)
                {
                    t.LookAt(this.transform);
                }
            }
        }
    }
}