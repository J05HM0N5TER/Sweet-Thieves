using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using XboxCtrlrInput;
public class PauseMenu : MonoBehaviour
{
#pragma warning disable IDE0044 // Add readonly modifier
	[SerializeField] private string mainMenuSceneName = "New Main Menu";
	[SerializeField] private XboxButton pauseButton = XboxButton.Start;
	public static bool isGamePaused = false;
    public GameObject PauseMenuUI;
#pragma warning restore IDE0044 // Add readonly modifier

    //the defualt button script must be on the panel and the buttons must be explicit 
    // Update is called once per frame
    void Update()
    {
		// If the player has pressed down the button to pause the game
        if (XCI.GetButtonDown(pauseButton))
        {
			// If the game is already paused
            if (isGamePaused)
            {
				// Resume the game
                Resume();
            }
			// If it is not already paused
            else
            {
				// Pause the game
                Pause();
            }
        }
        
    }

	/// <summary>
	/// For unpausing the game
	/// </summary>
    public void Resume()
    {
		// Hide UI
        PauseMenuUI.SetActive(false);
		// Resume time for game
        Time.timeScale = 1f;
		// Set to not paused
        isGamePaused = false;
    }

	/// <summary>
	/// For pausing the game
	/// </summary>
   void Pause()
    {
		// Show UI
        PauseMenuUI.SetActive(true);
		// Stops time for game
        Time.timeScale = 0f;
		// Set game to paused
        isGamePaused = true;
    }

	/// <summary>
	/// For going back to the main menu
	/// </summary>
    public void LoadMenu()
    {
        Debug.Log("loading menu");
		// Set time back to normal
        Time.timeScale = 1f;
		// Load main menu
        SceneManager.LoadScene(mainMenuSceneName); 
    }
    public void Quit()
    {
        Debug.Log("quit");
        Application.Quit();
    }
}
