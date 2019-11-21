using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    public GameObject player;
    private PlayerControllerXbox playerController;

    public GameObject Base;
    private BaseStash baseStashAmount;

    public float flickerTime = 0.5f;

    public int amountFlicker = 2;

    private float currentTimer = 0f;

    private bool firstTime = true;
    private bool playerFull = false;
    private int currentAmountFlickerd = 0;

    void Start()
    {
        playerController = player.GetComponent<PlayerControllerXbox>();
        baseStashAmount = Base.GetComponent<BaseStash>();
    }

    // Update is called once per frame
    void Update()
    {

        // Player is full
        if (playerController.heldCollectables >0) //playerController.maxHeldCollectables == playerController.heldCollectables)
        {
            // Set timer
            currentTimer += Time.deltaTime;
            playerFull = true;
            //gameObject.GetComponent<Light>().enabled = true;
            // If timer is up or its the first time (where it will do it constantly)
            if (currentTimer > flickerTime && (currentAmountFlickerd < amountFlicker || firstTime))
            {
                currentAmountFlickerd++;
                Light temp = gameObject.GetComponent<Light>();
                temp.enabled = !temp.enabled;
                currentTimer = 0f;
            }
        }
        else
        {
            // If the player dropped them off
            if (playerFull)
            {
                firstTime = false;
                currentAmountFlickerd = 0;
            }
            // Light on when no pancakes at base
            if (baseStashAmount.stashSize < 1)
            {
                gameObject.GetComponent<Light>().enabled = false;
            }
            else
            {
                gameObject.GetComponent<Light>().enabled = true;
            }
        }
    }
}
