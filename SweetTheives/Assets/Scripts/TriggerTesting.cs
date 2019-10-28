using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerTesting : MonoBehaviour
{
    public GameObject VenusFlyTrap;


    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
         
            //VenusFlyTrap.SetActive(false);
            VenusFlyTrap.transform.Rotate(0f, 0f, -103f);
            Debug.Log("rotated"); 
        }
    }
}
