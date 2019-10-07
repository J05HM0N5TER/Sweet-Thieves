using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject spawnee;
    public bool stopSpawning = false;
    public float spawnTime;
    public float spawnDelay;
    public int maxSpawn;
    int spawned;
    // Start is called before the frst frame update
    void Start()
    {
        spawned = 0;
        InvokeRepeating("SpawnObject", spawnTime, spawnDelay);
    }
    
    public void SpawnObject()
    {
        //for(int i = 0; i  <= maxSpawn; i ++)
        //{
           
        //}
        Instantiate(spawnee, transform.position, transform.rotation);
        spawned += 1;
        if (spawned == maxSpawn)
        {
            CancelInvoke("SpawnObject");
        }


        if (stopSpawning)
        {
            CancelInvoke("SpawnObject");
        }
    }
}
