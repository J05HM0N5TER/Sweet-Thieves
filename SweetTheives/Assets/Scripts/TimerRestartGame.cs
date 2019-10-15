using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using XboxCtrlrInput;


public class TimerRestartGame : MonoBehaviour
{
    //text to display the clock
    public Text clock = null;
    public float timer = 0.0f;

    public Text winner = null;

    [SerializeField] BaseStash player1 = null;
    [SerializeField] BaseStash player2 = null;
    [SerializeField] BaseStash player3 = null;
    [SerializeField] BaseStash player4 = null;
    //[SerializeField] PlayerControllerXbox player = null;
    [SerializeField] XboxButton selectbutton = XboxButton.A;
    [SerializeField] XboxButton exitbutton = XboxButton.B;
    [SerializeField] Canvas winscreen = null;

    // Start is called before the first frame update
    void Start()
    {
        winscreen.GetComponent<Canvas>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {

        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            timer = 0;
            //You can restart the scene here
            
        }
        int minutes = (int)(timer / 60);
        int seconds = (int)timer % 60;
        //if the seconds in the timer is 9 or less put a ":0"
        if (seconds <= 9)
        {
            clock.text = minutes.ToString() + ":0" + seconds.ToString();

        }
        //if the seconds is 0 put ":00"
        else if (seconds == 0)
        {
            clock.text = minutes.ToString() + ":00" ;

        }
        else
        {
            clock.text = minutes.ToString() + ":" + seconds.ToString();
        }
        if(timer == 0)
        {
            Time.timeScale = 0;
            float highestStack = 0;
            int winningPlayer = 0;
           
            
            if(player1.stashSize >= highestStack)
            {
                highestStack = player1.stashSize;
                winningPlayer = 1;
            }
            if(player2.stashSize > highestStack)
            {
                highestStack = player2.stashSize;
                winningPlayer = 2;
            }
            if (player3.stashSize > highestStack)
            {
                highestStack = player3.stashSize;
                winningPlayer = 3;
            }
            if (player4.stashSize > highestStack)
            {
                highestStack = player4.stashSize;
                winningPlayer = 4;
            }
            


            winner.text = "player " + winningPlayer + " Won!"; 
            winscreen.GetComponent<Canvas>().enabled = true;
            

            if (XCI.GetButton(selectbutton))
            {
                Time.timeScale = 1;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            if (XCI.GetButton(exitbutton))
            {
                Application.Quit();
            }
        }
        
        
    }

}
