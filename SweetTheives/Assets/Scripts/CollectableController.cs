using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableController : MonoBehaviour
{
    public int amount = 1;

    private void OnTriggerEnter(Collider other)
    {

        // Check if a collectable is being collected
        if (other.gameObject.tag == "Player")
        {
            PlayerControllerXbox script = other.gameObject.GetComponent<PlayerControllerXbox>();


            // Updates the amount of callectables that are being held
            // If the tongue is attached to this object then unset it.
            if (script.hitCollectable == this.gameObject)
            {
                script.tongueHitCollectible = false;
                script.hitCollectable = null;
            }
            script.heldCollectables += amount;
            // Distroy the collectable from the world
            Destroy(this.gameObject);
        }
    }
}
