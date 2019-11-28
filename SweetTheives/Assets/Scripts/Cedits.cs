using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cedits : MonoBehaviour
{
    [SerializeField] Canvas designers = null;
    [SerializeField] Canvas Artists_1 = null;
    [SerializeField] Canvas Artists_2 = null;
    [SerializeField] Canvas Programmers = null;
    [SerializeField] Canvas specialThanks = null;
    [SerializeField] Canvas teachers = null;
    [SerializeField] Canvas CodenameLemon = null;
    [SerializeField] float delay = 5.0f;
    private bool creditsPlaying = false;
	
    /// <summary>
	/// Checks if the credits need to be started
	/// </summary>
    void Update()
    {
		// If credits aren’t playing
        while(creditsPlaying == false)
        {
			// Start playing
            StartCoroutine(ItterateThroughCavases());
            creditsPlaying = true;
        }
    }

	/// <summary>
	/// Switches between the pages in the credits with delay
	/// </summary>
	/// <returns>Used for the WaitForSeconds function</returns>
    private IEnumerator ItterateThroughCavases()
    {
        // First page
		// Set page to display
        designers.gameObject.SetActive(true);
		// Leave it displayed for a delay
        yield return new WaitForSeconds(delay);
		// Hide page
        designers.gameObject.SetActive(false);
		// Second page
        Artists_1.gameObject.SetActive(true);
        yield return new WaitForSeconds(delay);
        Artists_1.gameObject.SetActive(false);
		// Third page
        Artists_2.gameObject.SetActive(true);
        yield return new WaitForSeconds(delay);
        Artists_2.gameObject.SetActive(false);
		// Forth page
        Programmers.gameObject.SetActive(true);
        yield return new WaitForSeconds(delay);
        Programmers.gameObject.SetActive(false);
		// Fifth page
        specialThanks.gameObject.SetActive(true);
        yield return new WaitForSeconds(delay);
        specialThanks.gameObject.SetActive(false);
		// Sixth page
        teachers.gameObject.SetActive(true);
        yield return new WaitForSeconds(delay);
        teachers.gameObject.SetActive(false);
		// Seventh page
        CodenameLemon.gameObject.SetActive(true);
		// Team name displayed longer then other pages
        yield return new WaitForSeconds(delay * 1.5f);
        CodenameLemon.gameObject.SetActive(false);
		// Set credits to no longer playing
        creditsPlaying = false;
    }
}
