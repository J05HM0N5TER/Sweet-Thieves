using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]

// this code is to allow UI functionality 
// it simply makes it so that there is a way to count how many pancakes 
// are currently in the stash

public class BasePoint : MonoBehaviour
{
#pragma warning disable IDE0044 // Add readonly modifier
    [HideInInspector] public bool PancakeStolen = false;
    //what stash is being referenced
    [SerializeField] BaseStash stash = null;

    // for playing sound when pancake is stolen
    private AudioSource audiosource;
	[SerializeField] private AudioClip BaseRaid = null;
#pragma warning restore IDE0044 // Add readonly modifier

	private void Start()
    {
        audiosource = GetComponent<AudioSource>();
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
