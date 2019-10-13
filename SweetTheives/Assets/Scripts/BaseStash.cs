using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseStash : MonoBehaviour
{
     public float stashSize;
    [SerializeField] GameObject player;
    [SerializeField] Spawner spawner;
    [SerializeField] float delay = 0.0f;
    // [SerializeField] GameObject spawnee;
    private void Start()
    {
        
    }

    private IEnumerator OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            PlayerControllerXbox player = other.gameObject.GetComponent<PlayerControllerXbox>();
            
            if (player == null )
            {
                Debug.Log("No script");
            }
            else
            {
                //Spawner spawner = other.gameObject.GetComponent<Spawner>();
               // stashSize += player.heldCollectables;
                spawner.maxSpawn += player.heldCollectables;
                if(player.heldCollectables != 0)
                {
                    for (int i = 0; i < player.heldCollectables; i++)
                    {
                        spawner.SpawnObject().GetComponent<Rigidbody>().constraints =
                          RigidbodyConstraints.FreezePositionX |
                          RigidbodyConstraints.FreezePositionZ |
                          RigidbodyConstraints.FreezeRotation;

                        yield return new WaitForSeconds(delay);
                    }
                   
                }
                player.heldCollectables = 0;
                spawner.maxSpawn = 0;



            }
           
        }


    }
}
