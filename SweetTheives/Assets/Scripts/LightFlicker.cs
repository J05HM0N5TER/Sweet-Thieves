using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightFlicker : MonoBehaviour
{
	// The player that is using this base
	public GameObject player;
	// The script on the player
	private PlayerControllerXbox playerController;

	// The gameObject for the base that the light is connected to
	public GameObject Base;
	// The script that is on the base that stores how many collectables is at the base
	private BaseStash baseStashAmount;

	// How fast it flickers
	public float flickerTime = 0.5f;

	// How much it flickers each timer the player picks up a collectable (Other then the first timer)
	public int amountFlicker = 2;

	// The current timer for how long since last light change
	private float currentTimer = 0f;

	// Is it the first time in the game since the player picked up a collectable
	private bool firstTime = true;
	// Is the player holding a collectable
	private bool playerHolding = false;
	// How many times has the light flickered since the player picked up anything
	private int currentAmountFlickerd = 0;
	// The light that is being controlled
    private Light flickeringLight = null;

	/// <summary>
	/// Cache the things that are expensive to call
	/// </summary>
	void Start()
	{
		playerController = player.GetComponent<PlayerControllerXbox>();
		baseStashAmount = Base.GetComponent<BaseStash>();
        flickeringLight = GetComponent<Light>();
	}

	/// <summary>
	/// Deals with all interaction
	/// </summary>
	void Update()
	{
        currentTimer += Time.deltaTime;
		// If the light should be flickering
		if (playerController.heldCollectables > 0 && (currentAmountFlickerd < amountFlicker || firstTime))
		{
            // Set timer
            playerHolding = true;
            if (currentTimer > flickerTime)
            {
                // If timer is up or its the first time (where it will do it constantly)
                currentAmountFlickerd++;
                flickeringLight.enabled = !flickeringLight.enabled;
                currentTimer = 0f;
            }
		}
		// If the light shouldn't be flickering
		else if (currentTimer > flickerTime)
		{
			// If the player dropped them off
			if (playerHolding && playerController.heldCollectables == 0)
			{
				firstTime = false;
				currentAmountFlickerd = 0;
			}
			// Light on when pancakes at base
			if (baseStashAmount.stashSize < 1)
			{
				flickeringLight.enabled = false;
			}
			else
			{
				flickeringLight.enabled = true;
			}
		}
	}
}
