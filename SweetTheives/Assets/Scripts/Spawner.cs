using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    //what the spawner will spawn
    [SerializeField] GameObject spawnee = null;
    //stop it from spawning in editor
    public bool stopSpawning = false;
    //will it continue to spawn 
    public bool repeating = false;
    //time until is starts spawning
    public float spawnTime = 0.0f;
    //time between spawns
    public float spawnDelay = 0.0f;
    //max amount it will spawn
    public int maxSpawn = 0;
    //amount that has been spawned
    int spawned = 0;
    // Start is called before the frst frame update
    void Start()
    {

        // initialize spawned objets to 0
        spawned = 0;

        // repeates the the spawning, starts after amount of seconds and then every amoun of time
        if (repeating == true)
        {
            InvokeRepeating("SpawnObject", spawnTime, spawnDelay);
        }
        
    }
    
    public GameObject SpawnObject()
    {
        
        // spawns the objects
        GameObject spawnedObject =  Instantiate(spawnee, transform.position, transform.rotation);
        

        // increase the value of spawned for each itteration    
        spawned += 1;

        // if the amount spawned is equal to the max allowed to spawn stop spawning
        if (spawned == maxSpawn)
        {
            CancelInvoke("SpawnObject");
        }

        // spawning is stopped if stop spawning box has been checked
        if (stopSpawning)
        {
            CancelInvoke("SpawnObject");
        }

        return spawnedObject;
    }
}
