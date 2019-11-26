using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using XboxCtrlrInput;




public class TimerRestartGame : MonoBehaviour
{
	//text to display the clock
	public Text clock = null;
	// The timer that counts down
	public double timer = 0.0f;

	// Where it is displaying the winner
	public Text winnerText = null;
	// The array of all the players
	private BaseStash[] playerBases = new BaseStash[4];
    //array of winning players
	public List<GameObject> winningPlayers = new List<GameObject>();

	// The canvas that is being displayed when the timer is up
	[SerializeField] Canvas winscreen = null;

	private bool finnished = false;
    //public GameObject WinPanel;
    //public GameObject LosePanel;
    public Image[] Losers;
    public Image[] Winners;
    //public Image BlueL;
    //public Image RedL;
    //public Image GreenL;
    //public Image YellowL;

    //public Image BlueW;
    //public Image RedW;
    //public Image GreenW;
    //public Image YellowW;


    // Start is called before the first frame update
    void Start()
	{
        playerBases = FindObjectsOfType<BaseStash>();

		//winscreen.GetComponent<Canvas>().enabled = false;
	}

	// Update is called once per frame
	void Update()
	{
		// If the winner has been found then don't execute code.
		if (finnished)
		{
			return;
		}

		// Count down timer
		timer -= Time.deltaTime;

		// Convert timer to display
		int minutes = (int)(timer / 60);
		int seconds = (int)timer % 60;
		//if the seconds in the timer is 9 or less put a ":0"
		if (seconds <= 9)
		{
			clock.text = minutes + ":0" + seconds;
		}
		//if the seconds is 0 put ":00"
		else if (seconds == 0)
		{
			clock.text = minutes.ToString() + ":00";

		}
		else
		{
			clock.text = minutes + ":" + seconds;
		}

		// If timer is finished
		if (timer <= 0)
		{


			// Stop all vibrations
			XCI.StopVibration(XboxController.All);

			// stop time
			Time.timeScale = 0;

			int highestStack = int.MinValue;

			// Loop though all the players and find the winner/s
			for (int i = 0; i < playerBases.Length; i++)
			{
				// And find who won
				if (playerBases[i].stashSize > highestStack)
				{
					highestStack = (int)playerBases[i].stashSize;

					// Reset winning players
					winningPlayers.Clear();
					// Set to current player
					winningPlayers.Add(playerBases[i].player.gameObject);
                 //  LosePanel.SetActive(false);
                  //  WinPanel.SetActive(true);
                    
				}
				else if (playerBases[i].stashSize == highestStack)
				{
					// Add current player to the list of winners
					winningPlayers.Add(playerBases[i].player.gameObject);
				}
			}
            
			// Create what to write as the winner/s
			string winString = "";
            
			for (int i = 0; i < winningPlayers.Count; i++)
			{
				if (i == 0)
				{
					winString = winningPlayers[i].name;
				}
				else
				{
					winString += " & " + winningPlayers[i].name;
				}
			}

			// Set the image for the winner
            for(int i = 0; i < winningPlayers.Count; i++)
            {
                int thisWinner =  winningPlayers[i].GetComponent<PlayerControllerXbox>().PortraitIndex;

                Losers[thisWinner].gameObject.SetActive(false);

                Winners[thisWinner].gameObject.SetActive(true);
            }


            // Set text to show who won
            //winscreen.GetComponent<Canvas>().enabled = true;
            winscreen.gameObject.SetActive(true);
			EventSystem.current.SetSelectedGameObject(null);
            //winnerText.text = winString + " Won!";
            finnished = true;
		}
	}
}
