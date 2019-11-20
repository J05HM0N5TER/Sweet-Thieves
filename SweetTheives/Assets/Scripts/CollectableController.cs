/* ---Requirements---
 * On collectable / parent game object:
 * - Renderer
 * - Capsule Collider:
 *      ~ Is trigger = true
 * - Rigidbody
 * - Mesh Collider
 * - Layer = Collectables
 * */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableController : MonoBehaviour
{
	public int stackSize = 1;

	/// <summary>
	/// Deals with interaction with players
	/// </summary>
	/// <param name="other">The GameObject that is colliding with the collectable</param>
	private void OnTriggerEnter(Collider other)
	{
		// If not a player then ignore interaction
		if (other.gameObject.tag != "Player")
		{
			return;
		}

		// Reference to the player script
		PlayerControllerXbox player = other.gameObject.GetComponent<PlayerControllerXbox>();
		if (player == null)
		{
			Debug.LogError("No script on player");
		}

		// If the players tongue is not attached
		if (player.objectHit != this.gameObject || player.tongueHit != HitType.COLLECTABLE)
		{
			// Ignore interaction
			return;
		}

		// Add to the amount that the player is holding
		player.heldCollectables += stackSize;
		// Unset from what the tongue is attached to
		player.objectHit = null;
		// Set the tongue interaction to none
		player.tongueHit = HitType.NONE;
		// Set cooldown to start
		player.currentCooldown = player.tongueCooldown;
		// Destroy collectable
		Destroy(this.gameObject);
        player.PlayPickUpSound();
	}
}
