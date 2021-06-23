using UnityEngine;

namespace NextMind.Examples.Utility
{
    /// <summary>
    /// This component ensures that the canvas attached to the same GameObject has the <see cref="Camera.main"/> set as world Camera.
    /// </summary>
    [RequireComponent(typeof(Canvas))]
    public class EnsureWorldCamera : MonoBehaviour
    {
        /// <summary>
        /// The canvas attached to the GameObject.
        /// </summary>
        private Canvas canvas;

        private void Awake()
        {
            canvas = GetComponent<Canvas>();

            // If the world camera is null set the main one.
            if (canvas.worldCamera == null)
            {
                canvas.worldCamera = Camera.main;
            }
        }
    }
}