using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableController : MonoBehaviour
{
    public int stackSize = 1;
    private void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            PlayerControllerXbox script = other.gameObject.GetComponent<PlayerControllerXbox>();
            if (script == null)
            {
                Debug.Log("No script");
            }
            script.heldCollectables += stackSize;
            if (script.objectHit == this.gameObject)
            {
                script.objectHit = null;
                script.tongueHitCollectible = false;
            }
            Destroy(this.gameObject);
        }
    }
}
