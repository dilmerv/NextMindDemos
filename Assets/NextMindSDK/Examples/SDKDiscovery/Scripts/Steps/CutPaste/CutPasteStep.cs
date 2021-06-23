using NextMind.Examples.Steps;
using NextMind.NeuroTags;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NextMind.Examples.Discovery
{
    public class CutPasteStep : AbstractStep
    {
        /// <summary>
        /// The zones where to paste the cut objects.
        /// </summary>
        [SerializeField]
        private List<PasteContainer> availablePositions = null;

        /// <summary>
        /// The objects to cut and paste.
        /// </summary>
        [SerializeField]
        private List<Transform> interactibleObjects = null;

        /// <summary>
        /// The currently selected object.
        /// </summary>
        private NeuroTag selectedObject;

        #region AbstractStep implementation

        public override void OnEnterStep()
        {
            base.OnEnterStep();

            // Place the objects on the first positions.
            for (int i = 0; i < interactibleObjects.Count; i++)
            {
                selectedObject = interactibleObjects[i].GetComponent<NeuroTag>();
                OnTriggerPosition(availablePositions[i]);
            }

            // Disactivate the positions.
            SetPositionsActive(false);
        }

        #endregion

        #region NeuroTag events 

        /// <summary>
        /// Called when a user focus on an object to cut it.
        /// </summary>
        /// <param name="neuroTag">The focused object</param>
        public void OnTriggerObject(NeuroTag neuroTag)
        {
            // Store the object.
            selectedObject = neuroTag;

            // Activate the zones so the user can paste the cut object.
            SetPositionsActive(true);

            // Make the object disappear by scaling it smoothly to zero.
            StartCoroutine(SmoothScale(false,selectedObject.transform));

            // Disactive all the objects NeuroTags.
            foreach (var c in interactibleObjects)
            { 
                c.GetComponent<NeuroTag>().enabled = false;
            }
        }

        /// <summary>
        /// Called when used focus on a zone, to paste the object he just cut.
        /// </summary>
        /// <param name="zone">The focused zone</param>
        public void OnTriggerPosition(PasteContainer zone)
        {
            // If we did not cut an object, leave the function.
            if (selectedObject == null)
            {
                return;
            }

            // Remove from old position.
            for (int i = 0; i < availablePositions.Count; i++)
            {
                if (availablePositions[i].containedGameObject == selectedObject.gameObject)
                {
                    availablePositions[i].OnChangeContent(null);
                    break;
                }
            }

            // Link the object to the chosen zone.
            zone.OnChangeContent(selectedObject.gameObject);
            
            // Disactivate all the zones.
            SetPositionsActive(false);

            // Make the pasted object reappear smoothly by scaling it up.
            StartCoroutine(SmoothScale(true, selectedObject.transform));

            // Active all the objects neurotags
            foreach (var c in interactibleObjects)
            {
                NeuroTag tag = c.GetComponent<NeuroTag>();
                if (tag != selectedObject)
                {
                    tag.enabled = true;
                }
            }

            // Active the NeuroTag on the pasted object after a few second. 
            // If it is reactivated directly, it can happen than the user focus on it and trigger it too quickly. 
            StartCoroutine(DelayedActivation(selectedObject));

            selectedObject = null;
        }

        #endregion

        /// <summary>
        /// Set the position active or inactive regarding their content.
        /// If true, activate only the free zones, which are not the one where we just cut object.
        /// </summary>
        /// <param name="active"></param>
        private void SetPositionsActive(bool active)
        {
            for (int i = 0; i < availablePositions.Count; i++)
            {
                PasteContainer pos = availablePositions[i];
                pos.gameObject.SetActive(active && (pos.IsFree || (selectedObject!=null && pos.containedGameObject == selectedObject.gameObject)));
            }
        }

        /// <summary>
        /// Smoothly scale up or down the <paramref name="target"/> pased in parameter.
        /// </summary>
        /// <param name="scaleUp">Scale up if true, scale down otherwise</param>
        /// <param name="target">The transform to scale</param>
        private IEnumerator SmoothScale(bool scaleUp, Transform target)
        {
            float duration = 0.3f;
            float timer = 0;
            float t = 0;

            AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

            Vector3 startScale = target.localScale;
            Vector3 targetScale = scaleUp ? 0.5f * Vector3.one : Vector3.zero;

            while (t < 1)
            {
                target.localScale = Vector3.Lerp(startScale,targetScale, curve.Evaluate(t));

                timer += Time.deltaTime;
                t = timer / duration;
                yield return null;
            }

            target.localScale = targetScale;
        }

        /// <summary>
        /// Active the tag passed in parameters after a fixed delay.
        /// </summary>
        /// <param name="tag">The tag to activate.</param>
        private IEnumerator DelayedActivation(NeuroTag tag)
        {
            yield return new WaitForSeconds(2f);

            tag.enabled = true;
        }
    }
}