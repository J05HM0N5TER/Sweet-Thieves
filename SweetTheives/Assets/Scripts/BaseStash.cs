using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BaseStash : MonoBehaviour
{

    //text to display the points
    public Text points = null;
    // size of current stash
     public float stashSize = 0;
    // what player is connected to base
    [SerializeField] Transform player = null;
    // what spawner is connected to base
    [SerializeField] Spawner spawner = null;
    // delay between spawning pancakes into base
    [SerializeField] float delay = 0.0f;


    //update function
    //set the score to stashsize;
    void Update()
    {
        points.text = stashSize.ToString();    
    }


    // on collision of base
    private IEnumerator OnTriggerEnter(Collider other)
    {
        //if it is the players base do code, if not nothing happens
        if (other.gameObject.transform == player)
        {
            //getting script from PlayerControllerXbox
            PlayerControllerXbox playercontrol = other.gameObject.GetComponent<PlayerControllerXbox>();
            
            //if no script found load error
            if (playercontrol == null )
            {
                Debug.Log("No script");
            }
            // otherwise
            else
            {
                // the max allowed to spawn in this instance is equal to how mnay pancakes the player is holding
                spawner.maxSpawn += playercontrol.heldCollectables;
                
                //if the player is holding more than 0
                if(playercontrol.heldCollectables != 0)
                {
                    //spawn the amount the player is holding with frozen transforms and a delay between spawns
                    for (int i = 0; i < playercontrol.heldCollectables; i++)
                    {
                        spawner.SpawnObject().GetComponent<Rigidbody>().constraints =
                          RigidbodyConstraints.FreezePositionX |
                          RigidbodyConstraints.FreezePositionZ |
                          RigidbodyConstraints.FreezeRotation;

                        yield return new WaitForSeconds(delay);
                    }
                   
                }
                // player loses how many is being held and the max spawn is returned to 0.
                playercontrol.heldCollectables = 0;
                spawner.maxSpawn = 0;

            }
           
        }


    }
}
