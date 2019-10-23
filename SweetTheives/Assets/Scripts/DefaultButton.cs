using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DefaultButton : MonoBehaviour
{
    // Start is called before the first frame update

   //this to make sure that there is always a button selected and 
   // so when the player presses the back button it defaults to the play button
    public GameObject defaultButton = null;
    void Start()
    {
        
    }
    //this to make sure that when the scene changes to the default button of that scence 
    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(defaultButton);
    }

    // Update is called once per frame
    void Update()
    {
        //this make sure that if a plyer clicks out it will back to the defualt button 
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(defaultButton);
        }
    }
}
