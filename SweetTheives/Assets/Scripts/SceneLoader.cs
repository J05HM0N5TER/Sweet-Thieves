using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using XboxCtrlrInput;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] XboxButton selectbutton = XboxButton.A;

    //public GameObject defaultButton = null;
    
    //public void PlayGame()
    //{
   //     SceneManager.LoadScene(1);

    //}
    public void PlayGame()
    {
        if (XCI.GetButton(selectbutton))
        {
            // this will add scences in the same order you put it in build settings
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
    public void Update()
    {
        //if(EventSystem.current.currentSelectedGameObject == null)
        //    EventSystem.current.SetSelectedGameObject(defaultButton);

        //    if (XCI.GetButton(selectbutton))
        //    {
        //        SceneManager.LoadScene(1);
        //    }
    }
    public void QuitGame()
    {
        Debug.Log("quit!");
        Application.Quit(); 
    }
}
