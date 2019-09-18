using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

public class PlayerControllerXbox : MonoBehaviour
{
    float xAxis = XCI.GetAxis(XboxAxis.LeftStickX, XboxController.First);
    float yAxis  = XCI.GetAxis(XboxAxis.LeftStickY, XboxController.First);

    
    //max move speeds
    public float maxMoveSpeed;
    public XboxController controller;
    public float dashSpeed;
    private Vector3 newPosition;

    //public Material matRed;
    //public Material matGreen;
    //public Material matBlue;
    //public Material matYellow;

    private static bool didQueryNumOfCtrlrs = false;

  

    // Start is called before the first frame update
    void Start()
    {
        //switch (controller)
        //{
        //    case XboxController.First: GetComponent<Renderer>().material = matRed; break;
        //    case XboxController.Second: GetComponent<Renderer>().material = matGreen; break;
        //    case XboxController.Third: GetComponent<Renderer>().material = matBlue; break;
        //    case XboxController.Fourth: GetComponent<Renderer>().material = matYellow; break;
        //}

        if (!didQueryNumOfCtrlrs)
        {
            didQueryNumOfCtrlrs = true;

            int queriedNumberOfCtrlrs = XCI.GetNumPluggedCtrlrs();
            if (queriedNumberOfCtrlrs == 1)
            {
                Debug.Log("Only " + queriedNumberOfCtrlrs + " Xbox controller plugged in.");
            }
            else if (queriedNumberOfCtrlrs == 0)
            {
                Debug.Log("No Xbox controllers plugged in!");
            }
            else
            {
                Debug.Log(queriedNumberOfCtrlrs + " Xbox controllers plugged in.");
            }

            XCI.DEBUG_LogControllerNames();
        }
       
    }

    // Update is called once per frame
    void Update()
    {

        if (XCI.GetButton(XboxButton.A, controller))
        {
            //dash
            newPosition = transform.position;
            float axisXDash = XCI.GetAxis(XboxAxis.LeftStickX, controller);
            float axisYDash = XCI.GetAxis(XboxAxis.LeftStickY, controller);
            float newPosXDash = newPosition.x + (axisXDash * dashSpeed * Time.deltaTime);
            float newPosZDash = newPosition.z + (axisYDash * dashSpeed * Time.deltaTime);
            newPosition = new Vector3(newPosXDash, transform.position.y, newPosZDash);
            transform.position = newPosition;
         
        }
        // movement from left joystick
        newPosition = transform.position;
        float axisX = XCI.GetAxis(XboxAxis.LeftStickX, controller);
        float axisY = XCI.GetAxis(XboxAxis.LeftStickY, controller);
        float newPosX = newPosition.x + (axisX * maxMoveSpeed * Time.deltaTime);
        float newPosZ = newPosition.z + (axisY * maxMoveSpeed * Time.deltaTime);
        newPosition = new Vector3(newPosX, transform.position.y, newPosZ);
        transform.position = newPosition;

        
        //var y = XCI.GetAxis(XboxAxis.LeftStickX, controller);
        ////transform.rotation = new Vector3(0, 0, 0);
        
        //transform.rotation = Quaternion.Euler(0, y, 0);

        float moveHorizontal = XCI.GetAxis(XboxAxis.LeftStickX, controller);
        float moveVertical = XCI.GetAxis(XboxAxis.LeftStickY, controller);
        if ( moveHorizontal != 0 || moveVertical != 0)
        {
            Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
            transform.rotation = Quaternion.LookRotation(movement);

            
        }
        


    }
    
}
