using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using XboxCtrlrInput;
public class PauseMenu : MonoBehaviour
{
    public static bool isGamePaused = false;
    public GameObject PauseMenuUI;

    //the defualt button script must be on the panel and the buttons must be explicit 
    // Update is called once per frame
    void Update()
    {
        if (XCI.GetButtonDown(XboxButton.Start))
        {
            if (isGamePaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
        
    }
    public void Resume()
    {
        PauseMenuUI.SetActive(false);
        Time.timeScale = 1f;// puts the time back to 1
        isGamePaused = false;
    }
   void Pause()
    {
        PauseMenuUI.SetActive(true);
        Time.timeScale = 0f; // stops the time 
        isGamePaused = true;
    }
    public void LoadMenu()
    {
        Debug.Log("loading menu");
        Time.timeScale = 1f;
        SceneManager.LoadScene("New Main Menu"); 
        //this string must match the name of the scene
    }
    public void Quit()
    {
        Debug.Log("quit");
        Application.Quit();
    }
}
