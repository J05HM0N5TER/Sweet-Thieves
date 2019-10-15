using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using XboxCtrlrInput;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] XboxButton selectbutton = XboxButton.A;
    //public void PlayGame()
    //{
   //     SceneManager.LoadScene(1);

    //}
    public void Update()
    {
       
        if (XCI.GetButton(selectbutton))
        {
            SceneManager.LoadScene(1);
        }
    }
    public void QuitGame()
    {
        Application.Quit(); 
    }
}
