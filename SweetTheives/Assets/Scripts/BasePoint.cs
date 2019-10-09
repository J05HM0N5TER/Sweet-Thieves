using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// this code is to allow UI functionality 
// it simply makes it so that there is a way to count how nmany pancakes 
// are currently in the stash

public class BasePoint : MonoBehaviour
{
    //what stash is being referenced
    [SerializeField] BaseStash stash = null;
    
    // on colliding increase stash size 
    private void OnTriggerEnter(Collider other)
    {
        stash.stashSize += 1;
        
    }

    //on leaving the collider stash size decreases and the pancake gains its collider back
    private void OnTriggerExit(Collider other)
    {
        stash.stashSize -= 1;
        other.GetComponent<CapsuleCollider>().enabled = true;
    }
    
}
