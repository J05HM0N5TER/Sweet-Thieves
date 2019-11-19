using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]

// this code is to allow UI functionality 
// it simply makes it so that there is a way to count how many pancakes 
// are currently in the stash

public class BasePoint : MonoBehaviour
{
    [HideInInspector] public bool PancakeStolen = false;
    //what stash is being referenced
    [SerializeField] BaseStash stash = null;
    AudioSource audiosource;
    [SerializeField] AudioClip BaseRaid = null;

    // on colliding increase stash size 
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag != "Player")
        {  
           // stash.stashSize += 1;
        }
       
        
    }

    //on leaving the collider stash size decreases and the pancake gains its collider back
    private void OnTriggerExit(Collider other)
    {
        if (other.tag != "Player")
        {
            stash.stashSize -= 1;
            other.GetComponent<CapsuleCollider>().enabled = true;
            audiosource.PlayOneShot(BaseRaid, 1.0f);
        }
            
    }
   
}
