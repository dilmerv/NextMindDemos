using UnityEngine;

namespace NextMind.Examples.Utility
{
    /// <summary>
    /// The MouseCursorEnabler is used to automatically hide the cursor after <see cref="visibleTimeAfterlastMove"/> seconds of inactivity. 
    /// The cursor is displayed back as soon as the mouse moves.
    /// </summary>
    public class MouseCursorEnabler : MonoBehaviour
    {
        /// <summary>
        /// The time of inactivity allowed before the cursor disappear
        /// </summary>
        private readonly float visibleTimeAfterlastMove = 4f;

        private float timer = 0f;

        void Update()
        {
            if (HasMouseMoved())
            {
                timer = 0f;
                Cursor.visible = true;
            }

            if (Cursor.visible)
            {
                timer += Time.deltaTime;
            }

            if (timer > visibleTimeAfterlastMove)
            {
                Cursor.visible = false;
            }
        }

        /// <summary>
        /// Check if the mouse is moving this frame.
        /// </summary>
        private bool HasMouseMoved()
        {
            return !Mathf.Approximately(Input.GetAxis("Mouse X"),0) || !Mathf.Approximately(Input.GetAxis("Mouse Y"), 0);
        }
    }
}