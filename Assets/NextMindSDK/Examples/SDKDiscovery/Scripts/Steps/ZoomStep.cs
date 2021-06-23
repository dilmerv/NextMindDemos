using NextMind.Examples.Steps;
using NextMind.NeuroTags;
using System.Collections;
using UnityEngine;

namespace NextMind.Examples.Discovery
{
    public class ZoomStep : AbstractStep
    {
        private Camera zoomCamera;
        private float originalFOVValue;
        private Quaternion originalRotation;

        [SerializeField]
        private float zoomedFOVValue = 0;

        private Coroutine zoomCoroutine;

        private Transform followedObject;

        private bool unzooming = false;

        [SerializeField]
        private AnimationCurve cameraLockCurve = null;
        private float cameraLockRatio = 0;

        #region AbstractStep implementation

        public override void OnEnterStep()
        {
            base.OnEnterStep();

            if (zoomCamera == null)
            {
                zoomCamera = Camera.main;
            }

            originalFOVValue = zoomCamera.fieldOfView;
            originalRotation = zoomCamera.transform.rotation;
        }

        public override void UpdateStep()
        {
            base.UpdateStep();

            if (!unzooming && followedObject != null)
            {
                Quaternion lookRot = Quaternion.LookRotation(followedObject.position - zoomCamera.transform.position, Vector3.up);

                zoomCamera.transform.rotation = Quaternion.Lerp(zoomCamera.transform.rotation, lookRot, cameraLockRatio * Time.deltaTime);
            }
        }

        public override void OnExitStep()
        {
            base.OnExitStep();

            zoomCamera.fieldOfView = originalFOVValue;
            zoomCamera.transform.rotation = originalRotation;
        }

        #endregion

        #region NeuroTag events

        public void OnTriggeredFarObject(GameObject farObject)
        {
            followedObject = farObject.transform;

           StartCoroutine(OnZoom(true));
        }

        public void OnReleasedFarObject(GameObject farObject)
        {
            StartCoroutine(OnZoom(false));
        }

        #endregion

        private IEnumerator OnZoom(bool zoom)
        {
            if (zoomCoroutine != null)
            {
                StopCoroutine(zoomCoroutine);
            }

            unzooming = !zoom;

            zoomCoroutine = StartCoroutine(SmoothZoom(zoom));

            yield return zoomCoroutine;

            unzooming = false;

            if (!zoom)
            {
                followedObject = null;
            }
        }

        private IEnumerator SmoothZoom(bool zoom)
        {
            float t = 0f;
            float duration = 0.5f;
            float currentTimer = 0f;

            float startValue = zoomCamera.fieldOfView;
            float targetValue = zoom ? zoomedFOVValue : originalFOVValue;

            float startLockRatio = 0;
            float targetLockRatio = 5;

            Renderer followedRenderer = followedObject.GetComponent<NeuroTag>().StimulationRenderers[0].GetComponent<Renderer>();
            float startDensity = followedRenderer.material.GetFloat("_Density");
            float targetDensity = zoom? 8:4;

            Quaternion startRotation = zoomCamera.transform.rotation;

            while (t < 1)
            {
                zoomCamera.fieldOfView = Mathf.Lerp(startValue, targetValue, t);

                if (!zoom)
                {
                    zoomCamera.transform.rotation = Quaternion.Lerp(startRotation, originalRotation, t);
                }
                else
                {
                    cameraLockRatio = Mathf.Lerp(startLockRatio, targetLockRatio, cameraLockCurve.Evaluate(t));
                }

                followedRenderer.material.SetFloat("_Density", Mathf.Lerp(startDensity, targetDensity, t));

                currentTimer += Time.deltaTime;
                t = currentTimer / duration;
                yield return null;
            }

            zoomCamera.fieldOfView = targetValue;
            cameraLockRatio = targetLockRatio;
            followedRenderer.material.SetFloat("_Density", targetDensity);

            zoomCoroutine = null;
        }
    }
}