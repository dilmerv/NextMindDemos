using NextMind.Examples.Steps;
using System.Collections;
using UnityEngine;

namespace NextMind.Examples.Discovery
{
    public class ExplosionStep : AbstractStep
    {
        [SerializeField]
        private Rigidbody[] rigidbodies = null;

        [SerializeField]
        private Transform[] explosionPoints = null;

        [SerializeField]
        private ParticleSystem explosion = null;

        private struct InitialValues
        {
            public Vector3 position;
            public Quaternion rotation;
        }
        private InitialValues[] initialValues;

        private bool hasExplode;

        private bool trigged;

        #region AbstractStep implementation

        public override void OnEnterStep()
        {
            base.OnEnterStep();

            initialValues = new InitialValues[rigidbodies.Length];
            for (int i = 0; i < initialValues.Length; i++)
            {
                initialValues[i].position = rigidbodies[i].position;
                initialValues[i].rotation = rigidbodies[i].rotation;
            }
        }

        #endregion

        #region NeuroTag events

        public void OnTrigger(int zoneIndex)
        {
            if (trigged || hasExplode)
            {
                return;
            }

            StartCoroutine(ExplodeAfterTimer(zoneIndex));
        }

        #endregion

        private IEnumerator ExplodeAfterTimer(int zoneIndex)
        {
            trigged = true;

            explosion.transform.position = explosionPoints[zoneIndex].position;
            explosion.Play();

            for (int i = 0; i < rigidbodies.Length; i++)
            {
                rigidbodies[i].AddExplosionForce(150, explosionPoints[zoneIndex].position, 10);
            }

            hasExplode = true;
            trigged = false;
            StartCoroutine(AfterExplosionCoroutine(zoneIndex));
            yield return null;
        }


        private IEnumerator AfterExplosionCoroutine(int indexToReset)
        {
            yield return new WaitForSeconds(3f);

            for (int i = 0; i < rigidbodies.Length; i++)
            {
                rigidbodies[i].position = initialValues[i].position;
                rigidbodies[i].rotation = initialValues[i].rotation;
            }

            hasExplode = false;
        }
    }
}