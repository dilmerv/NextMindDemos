using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace NextMind.Examples.Calibration
{
    /// <summary>
    /// This component is used to make an Image switch between two colours following time parameters.
    /// It used for to animate the LED elements on pairing instructions schemes.
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class SchemeLedBlinker : MonoBehaviour
    {
        [SerializeField]
        private float waitBetweenBlinks;
        [SerializeField]
        private float blinkDuration;

        [SerializeField]
        private Color onColor;
        [SerializeField]
        private Color offColor;

        private Image image;

        // Start is called before the first frame update
        void Start()
        {
            image = GetComponent<Image>();

            StartCoroutine(Blink());
        }

        // Update is called once per frame
        private IEnumerator Blink()
        {
            while (true)
            {
                image.color = offColor;
                yield return new WaitForSeconds(waitBetweenBlinks);
                image.color = onColor;
                yield return new WaitForSeconds(blinkDuration);
            }
        }
    }
}