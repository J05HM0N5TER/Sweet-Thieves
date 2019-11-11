/* ---Requirements--- have you turned it off and on again?
    This code has to be applied to a box collider which is a trigger 
    that is around the button, not on the button itself.
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VenusButton : MonoBehaviour
{
    // the player that can press the button
    [SerializeField] GameObject Player = null;
    //what base is the venusfly trap prtecting
    [SerializeField] GameObject PlayerBase = null;
    //how big the radius is that players will be respawned from
    [SerializeField] float distroyRadius = 0.0f;
    //the venusfly trap that will be animated.
    [SerializeField] Animator venusAnim = null;

   // when something collides with the button
    private void OnTriggerEnter(Collider other)
    {
        // if it is the player that owns the button
        if(other.gameObject == Player)

        {
            // array of all things colliding with the base
            Collider[] hitColliders = Physics.OverlapSphere(PlayerBase.transform.position, distroyRadius);
            //for each thing that is colliding with the base itterate 
            foreach (Collider current in hitColliders)
            {
                //if one of the things is a player then respawn them
                if (current.gameObject.tag == "Player")
                {
                    PlayerControllerXbox playercontrol = current.gameObject.GetComponent<PlayerControllerXbox>();
                    playercontrol.ResetToSpawn();
                    
                }
            }
            //venusAnim.SetBool("venusBite", true);
            venusAnim.SetTrigger("venusBite");
            
        }
        
    }//on exit reset venus bite bool to false to go back to idle (have to do this so that the animation can finish)
    private void OnTriggerExit(Collider other)
    {
        venusAnim.ResetTrigger("venusBite");
    }
}
