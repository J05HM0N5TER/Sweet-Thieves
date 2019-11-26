using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using XboxCtrlrInput;
using UnityEngine.UI;

public class DefaultButton : MonoBehaviour
{
    // Start is called before the first frame update

    //this to make sure that there is always a button selected and 
    // so when the player presses the back button it defaults to the play button
    //

    float controlTimer = 0.2f;

    public GameObject defaultButton = null;
    void Start()
    {
        
    }
    //this to make sure that when the scene changes to the default button of that scence 
    private void OnEnable()
    {
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(defaultButton);
        }
    }

    // Update is called once per frame
    void Update()
    {
        controlTimer += Time.unscaledDeltaTime;

        //this make sure that if a plyer clicks out it will back to the defualt button 
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(defaultButton);
        }

        if (controlTimer > 0.2f)
        {
            if (XCI.GetAxis(XboxAxis.LeftStickY) < -0.5f)
            {
                GameObject selected = EventSystem.current.currentSelectedGameObject;
                if (selected)
                {
                    Selectable selectable = selected.GetComponent<Selectable>();
                    if (selectable)
                    {
                        Navigation nav = selectable.navigation;
                        if (nav.selectOnDown != null)
                        {
                            EventSystem.current.SetSelectedGameObject(nav.selectOnDown.gameObject);
                            controlTimer = 0.0f;
                        }
                    }
                }
            }
            if (XCI.GetAxis(XboxAxis.LeftStickY) > 0.5f)
            {
                GameObject selected = EventSystem.current.currentSelectedGameObject;
                if (selected)
                {
                    Selectable selectable = selected.GetComponent<Selectable>();
                    if (selectable)
                    {
                        Navigation nav = selectable.navigation;
                        if (nav.selectOnUp != null)
                        {
                            EventSystem.current.SetSelectedGameObject(nav.selectOnUp.gameObject);
                            controlTimer = 0.0f;
                        }
                    }
                }
            }
        }

        if (XCI.GetButtonDown(XboxButton.A))
        {
            GameObject selected = EventSystem.current.currentSelectedGameObject;
            if (selected)
            {
                Button selectable = selected.GetComponent<Button>();
                if (selectable)
                {
                    selectable.onClick.Invoke();
                }
            }
        }
    }

}
