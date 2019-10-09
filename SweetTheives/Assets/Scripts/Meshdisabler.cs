using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//disables collider of pancake when spawned so that it does not get counted twice in the stash

public class Meshdisabler : MonoBehaviour
{
    // on collision disable collider
    private void OnTriggerEnter(Collider other)
    {
        other.GetComponent<CapsuleCollider>().enabled = false;
    }
   
}
