using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VenusButton : MonoBehaviour
{

    [SerializeField] GameObject Player = null;
    [SerializeField] GameObject PlayerBase = null;
    [SerializeField] float distroyRadius = 0.0f; 
    

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == Player)
        { 
            Collider[] hitColliders = Physics.OverlapSphere(PlayerBase.transform.position, distroyRadius);
            foreach (Collider current in hitColliders)
            {
                if (current.gameObject.tag == "Player")
                {
                    PlayerControllerXbox playercontrol = current.gameObject.GetComponent<PlayerControllerXbox>();
                    playercontrol.ResetToSpawn();
                }
            }     
        }
    }
}
