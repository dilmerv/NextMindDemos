using NextMind.Examples.Steps;
using UnityEngine;

namespace NextMind.Examples.Calibration
{
    /// <summary>
    /// Implementation of an <see cref="AbstractStep"/> managed by the <see cref="StepsManager"/>.
    /// During this step, a welcome message is shown to the user. 
    /// Nothing to override from <see cref="AbstractStep"/>, the default behaviour is enough.
    /// </summary>
    public class IntroStep : AbstractStep
    {
        /// <summary>
        /// Triggered when user clicks on an hypertext link.
        /// </summary>
        /// <param name="link">The link to open</param>
        public void OnClickOnLink(string link)
        {
            Application.OpenURL(link);
        }
    }
}