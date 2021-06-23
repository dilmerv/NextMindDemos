using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NextMind.Examples
{
    /// <summary>
    /// This MonoBehaviour manage the hub scene behaviours, and the loading of the examples scenes.
    /// </summary>
    public class HubManager : MonoBehaviour
    {
        #region Singleton 

        /// <summary>
        /// Ensure that we have only one NeuroManager.
        /// </summary>
        private static HubManager instance;

        public static HubManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<HubManager>();
                    if (instance == null)
                    {
                        var go = new GameObject("HubManager");
                        instance = go.AddComponent<HubManager>();
                    }
                }

                return instance;
            }
        }

        #endregion

        /// <summary>
        /// The name of the loaded scene.
        /// </summary>
        private static string loadedScene = string.Empty;

        /// <summary>
        /// The panel displaying the scenes choice to the user.
        /// </summary>
        [SerializeField]
        private GameObject hubPanel = null;

        /// <summary>
        /// Environments lights that can be tweaked at runtime, calling <see cref="SetLights(bool)"/>.
        /// </summary>
        [SerializeField]
        private Light[] tweakableLights = null;
        private float[] originalIntensities;

        private void Awake()
        {
            Application.targetFrameRate = 90;

            // Get the original intensities.
            originalIntensities = new float[tweakableLights.Length];
            for (int i = 0; i < tweakableLights.Length; i++)
            {
                originalIntensities[i] = tweakableLights[i].intensity;
            }
        }

        private void Update()
        {
            // Exit application if the hub is the only loaded scene.
            if (Input.GetKeyDown(KeyCode.Escape) && loadedScene == string.Empty)
            {
                Application.Quit();
            }
        }

        /// <summary>
        /// A public function allowing to return to the hub from the example scenes.
        /// </summary>
        public void BackToHubScene()
        {
            if (!string.IsNullOrEmpty(loadedScene))
            {
                SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName(loadedScene));
            }

            loadedScene = string.Empty;

            hubPanel.SetActive(true);
        }

        /// <summary>
        /// Unload the current scene if it exists, then load additively a new one.
        /// </summary>
        /// <param name="sceneName"></param>
        public void LoadScene(string sceneName)
        {
            hubPanel.SetActive(false);

            if (!string.IsNullOrEmpty(loadedScene))
            {
                SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName(loadedScene));
            }

            loadedScene = sceneName;
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        }

        /// <summary>
        /// A function used to light up/down the referenced lights when needed.
        /// </summary>
        public IEnumerator SetLights(bool turnOn)
        {
            // Get the starting intensities of the referenced lights.
            float[] startIntensities = new float[tweakableLights.Length];
            for (int i = 0; i < tweakableLights.Length; i++)
            {
                startIntensities[i] = tweakableLights[i].intensity;
            }

            // Get the target intensities of the referenced lights.
            float[] targetIntensities = new float[tweakableLights.Length];
            for (int i = 0; i < tweakableLights.Length; i++)
            {
                targetIntensities[i] = turnOn ? originalIntensities[i] : originalIntensities[i] * 0.4f;
            }

            float t = 0;
            // Turn on the lights faster than turning them off.
            float duration = turnOn ? 0.5f : 2f;
            float timer = 0f;

            AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

            while (t < 1)
            {
                for (int i = 0; i < tweakableLights.Length; i++)
                {
                    tweakableLights[i].intensity = Mathf.Lerp(startIntensities[i], targetIntensities[i], curve.Evaluate(t));
                }

                timer += Time.deltaTime;
                t = timer / duration;

                yield return null;
            }

            for (int i = 0; i < tweakableLights.Length; i++)
            {
                tweakableLights[i].intensity = targetIntensities[i];
            }
        }
    }
}