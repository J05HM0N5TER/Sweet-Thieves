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
    private Light light;

	void Start()
	{
		playerController = player.GetComponent<PlayerControllerXbox>();
		baseStashAmount = Base.GetComponent<BaseStash>();
        light = GetComponent<Light>();
	}

	// Update is called once per frame
	void Update()
	{

        currentTimer += Time.deltaTime;
		// Player is holding pancakes
		if (playerController.heldCollectables > 0 && (currentAmountFlickerd < amountFlicker || firstTime))
		{
            // Set timer
            playerFull = true;
            if (currentTimer > flickerTime)
            {
                // If timer is up or its the first time (where it will do it constantly)
                currentAmountFlickerd++;
                light.enabled = !light.enabled;
                currentTimer = 0f;
            }
		}
		else if (currentTimer > flickerTime)
		{
			// If the player dropped them off
			if (playerFull && playerController.heldCollectables == 0)
			{
				firstTime = false;
				currentAmountFlickerd = 0;
			}
			// Light on when no pancakes at base
			if (baseStashAmount.stashSize < 1)
			{
				light.enabled = false;
			}
			else
			{
				light.enabled = true;
			}
		}
	}
}
