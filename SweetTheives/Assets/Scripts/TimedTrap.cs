using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedTrap : MonoBehaviour
{
	/* Where the attack is based around is relation to the gameObject 
	 *	(direction follows the facing of the gameObject)*/
	public Vector3 attackOffset;
	// How big the attack area is
	public float attackRadius = 0;
	// How often it attacks (in seconds)
	public float attackCooldown = 0;
	// What the current countdown is
	private float currentCooldown = 0;

	// Update is called once per frame
	void Update()
	{
		// Countdown timer
		currentCooldown -= Time.deltaTime;

		// If countdown is completed
		if (currentCooldown < 0)
		{
			// Calculate position that attack is taking place
			Vector3 attackPoint = transform.TransformPoint(attackOffset);

			// Get everything in that area
			Collider[] inAttackRange = Physics.OverlapSphere(attackPoint, attackRadius);
			foreach (Collider current in inAttackRange)
			{
				// If the thing is a player
				if (current.tag == "Player")
				{
					PlayerControllerXbox player = current.GetComponent<PlayerControllerXbox>();
					// Get the player to drop all collectables
					player.DropCollectables();
					// And send them back to spawn
					player.ResetToSpawn();
				}
			}
			currentCooldown = attackCooldown;
		}
	}
}
