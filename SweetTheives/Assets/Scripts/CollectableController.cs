using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableController : MonoBehaviour
{
    public int stackSize = 1;
    private GameObject playerSpawnedFrom = null;
    private void Start()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            PlayerControllerXbox player = other.gameObject.GetComponent<PlayerControllerXbox>();
            if (player == null)
            {
                Debug.Log("No script");
            }
            if (player.stolenCollectable == gameObject || player.heldCollectables >= player.maxHeldCollectables)
            {
                //script.stolenCollectable = null;
                return;
            }
            else
            {
                player.heldCollectables += stackSize;
                if (player.objectHit == gameObject)
                {
                    player.objectHit = null;
                    player.tongueHitCollectible = false;
                    player.currentCooldown = player.tongueCooldown;
                }
                Destroy(gameObject);
            }

        }
    }
}
