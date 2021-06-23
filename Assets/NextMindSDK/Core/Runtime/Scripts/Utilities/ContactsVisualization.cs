using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NextMind.Devices
{
    /// <summary>
    /// This component displays the reliablity of the <see cref="Device"/>'s contacts.
    /// </summary>
    /// <remarks>
    /// It will display contact values as colors on the <see cref="Image"/> component of the children objects. To map a contact's <see cref="Image"/> to its value, children must have specific names.
    /// The following table shows the relation between electrodes names and their id:
    /// <list type="table">
    /// <listheader>
    ///     <term>Name</term>
    ///     <description>Id</description>
    /// </listheader>
    /// <item><term>PO8</term><description>0</description></item>
    /// <item><term>PO4</term><description>1</description>
    /// </item>
    /// <item>
    ///     <term>O2</term><description>2</description>
    /// </item>
    /// <item>
    ///     <term>POZ</term><description>3</description>
    /// </item>
    /// <item>
    ///     <term>O1</term><description>4</description>
    /// </item>
    /// <item>
    ///     <term>OZ</term><description>5</description>
    /// </item>
    /// <item>
    ///     <term>PO3</term><description>6</description>
    /// </item>
    /// <item>
    ///     <term>PO7</term><description>7</description>
    /// </item>
    /// </list>
    /// If you intend to get the contacts values by yourself, you may want to use the <see cref="Device.GetContact(uint)"/> method with these ids as parameter.
    /// </remarks>
    public class ContactsVisualization : MonoBehaviour
    {
        /// <summary>
        /// The color to apply on a electrode in a neutral state (e.g. not on the user's head).
        /// </summary>
        [SerializeField]
        private Color neutralColor = Color.grey;

        /// <summary>
        /// The gradient coloring the electrdodes from bad contact to perfect contact.
        /// </summary>
        [SerializeField]
        private Gradient colorGradient;

        /// <summary>
        /// The list of electrodes names.
        /// </summary>
        private List<string> electrodesNames = new List<string> { "PO8", "PO4", "O2", "POZ", "O1", "OZ", "PO3", "PO7" };

        private List<Image> electrodesImage = new List<Image>();

        /// <summary>
        /// The device currently connected.
        /// </summary>
        private Device connectedDevice;

        /// <summary>
        /// The last batch of contact values
        /// </summary>
        private Dictionary<string, float> contactValues = new Dictionary<string, float>();

        /// <summary>
        /// One lerp coroutine for each contact.
        /// </summary>
        private Coroutine[] colorLerpCoroutines;

        /// <summary>
        /// Are we currently updating the contacts?
        /// </summary>
        private bool updatingContacts = false;

        private void OnEnable()
        {
            contactValues = new Dictionary<string, float>();
            electrodesImage = new List<Image>();

            // Get the electrodes Image components from their GameObject name. 
            for (int i = 0; i < electrodesNames.Count; i++)
            {
                contactValues.Add(electrodesNames[i], 0);
                foreach (Transform t in transform)
                {
                    if (t.name == electrodesNames[i])
                    {
                        electrodesImage.Add(t.GetComponent<Image>());
                        break;
                    }
                }
            }

            colorLerpCoroutines = new Coroutine[electrodesImage.Count];

            // Refresh contact 25 times per second.
            StartUpdateContactValues(25f);
        }

        private void OnDisable()
        {
            updatingContacts = false;
        }

        private void StartUpdateContactValues(float rate)
        {
            if (rate == 0)
            {
                updatingContacts = false;
            }
            else
            {
                updatingContacts = true;
                StartCoroutine(RefreshContactValues(1f / rate));
            }
        }

        private IEnumerator RefreshContactValues(float rate)
        {
            while (updatingContacts)
            {
                if (connectedDevice != null && connectedDevice.IsConnected)
                {
                    for (int i = 0; i < electrodesNames.Count; i++)
                    {
                        float contactValue = connectedDevice.GetContact((uint)i) / 100f;

                        // If the contact value changed.
                        if (!Mathf.Approximately(contactValues[electrodesNames[i]], contactValue))
                        {
                            contactValues[electrodesNames[i]] = contactValue;

                            if (colorLerpCoroutines[i] == null)
                            {
                                Color targetColor;
                                float value = contactValues[electrodesNames[i]];
                                if (value < 0 || value > 1)
                                {
                                    targetColor = neutralColor;
                                }
                                else
                                {
                                    targetColor = colorGradient.Evaluate(value);
                                }

                                electrodesImage[i].color = targetColor;
                            }
                        }
                    }
                }
                else
                {
                    // Retrieve the first connected device.
                    var connectedDevices = NeuroManager.Instance.ConnectedDevices;
                    for (int i = 0; i < connectedDevices.Count; i++)
                    {
                        Device device = connectedDevices[i];
                        if (device.IsConnected)
                        {
                            connectedDevice = device;
                            break;
                        }
                    }
                }

                yield return new WaitForSeconds(rate);
            }
        }
    }
}