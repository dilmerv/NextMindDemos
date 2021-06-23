using UnityEngine;

namespace NextMind.Examples.Discovery
{
    public class PasteContainer : MonoBehaviour
    {
        public GameObject containedGameObject;

        [SerializeField]
        private GameObject triangleFeedback = null;

        [SerializeField]
        private Transform objectAnchor = null;

        public bool IsFree => containedGameObject == null;

        public void OnChangeContent(GameObject pasted)
        {
            containedGameObject = pasted;
            if (pasted != null)
            {
                pasted.transform.position = objectAnchor.position;
                pasted.transform.rotation = objectAnchor.rotation;
            }
        }

        public void SetActive(bool active)
        {
            GetComponent<Renderer>().enabled = active;
            triangleFeedback.SetActive(active);
        }
    }
}