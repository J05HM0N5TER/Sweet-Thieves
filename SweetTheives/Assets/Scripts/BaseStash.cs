using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BaseStash : MonoBehaviour
{
#pragma warning disable IDE0044 // Add readonly modifier

    //text to display the points
    public Text points = null;
    // size of current stash
    public float stashSize = 0;
    // what player is connected to base
    public Transform player = null;
    // what spawner is connected to base
    [SerializeField] Spawner spawner = null;

	// delay between spawning pancakes into base
	[SerializeField] private float delay = 0.0f;

	[SerializeField] GameObject spawnee = null;
    //stop it from spawning in editor
    [SerializeField] bool stopSpawning = false;
    //will it continue to spawn 
    [SerializeField] bool repeating = false;
    //time until is starts spawning
    public float spawnTime = 0.0f;
    //time between spawns
    public float spawnDelay = 0.0f;
    //max amount it will spawn
    public int maxSpawn = 0;
    //amount that has been spawned
    int spawned = 0;

	public float spawneeHeight;
#pragma warning restore IDE0044 // Add readonly modifier

	/// <summary>
	/// Used to cache expensive calls
	/// </summary>
	private void Start()
	{
		// Get the height of the collectable to use to stack them later
		spawneeHeight = spawnee.GetComponent<CapsuleCollider>().bounds.size.y;
	}

	/// <summary>
	/// Updates the display for the amount of collectables at the base
	/// </summary>
	void Update()
    {
        points.text = stashSize.ToString();
    }


    /// <summary>
	/// Deals with interaction between player and base
	/// </summary>
	/// <param name="other">The collider that is colliding with the gameObject</param>
	/// <returns>Used for the WaitForSeconds function</returns>
    private IEnumerator OnTriggerEnter(Collider other)
    {

        //if it is the players base do code, if not nothing happens
        if (other.gameObject.transform == player)
        {
            //getting script from PlayerControllerXbox
            PlayerControllerXbox playercontrol = other.gameObject.GetComponent<PlayerControllerXbox>();
            int collectablesHeld = playercontrol.heldCollectables;
            playercontrol.heldCollectables = 0;

            //if no script found load error
            if (playercontrol == null)
            {
                Debug.Log("No script");
            }
            // otherwise
            else
            {
                // the max allowed to spawn in this instance is equal to how many pancakes the player is holding
                spawner.maxSpawn += collectablesHeld;

                //spawn the amount the player is holding with frozen transforms and a delay between spawns
                for (int i = 0; i < collectablesHeld; i++)
                {
                    SpawnObject().GetComponent<Rigidbody>().constraints =
                        RigidbodyConstraints.FreezePositionX |
                        RigidbodyConstraints.FreezePositionZ |
                        RigidbodyConstraints.FreezeRotation;

                    yield return new WaitForSeconds(delay);
                }
                // the max spawn is returned to 0.
                spawner.maxSpawn = 0;
            }
        }
    }

	/// <summary>
	/// Spawns a new collectable at the position of that gameObject.
	/// </summary>
	/// <returns>The collectable that was spawned</returns>
    public GameObject SpawnObject()
    {
        // spawns the objects
        GameObject spawnedObject = Instantiate(spawnee, new Vector3(gameObject.transform.position.x, spawned * spawneeHeight, gameObject.transform.position.z), transform.rotation);
        spawnedObject.GetComponent<CapsuleCollider>().enabled = false;

        // increase the value of spawned for each iteration    
        spawned += 1;
        stashSize++;

        return spawnedObject;
    }
}
