using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public LayerMask environmentLayer;
    public LayerMask collectableLayer;
    // Movement of character
    //public float speed = 20;
    //public float rotateSpeed = 40;
    // The postion for the hit on the tongue on a wall
    [HideInInspector]
    public Vector3 tongueHit;
    public float timeToGrappleCompletion = 1;

    // How close to the collision position for the tongue it has to be for it to release
    public float acceptanceRange = 0.5f;
    // The amount that the player is moving by the tongue every frame
    private Vector3 tongueGrappleMovement;
    // The radus for the sphere cast
    public float sphereCastRadus = 5f;
    // The amout of time that it has to retract in frames
    private float timeLeftToRetract;

    // If the tongue is currently retracting
    private bool tongueHitEnvionment = false;
    [HideInInspector]
    public bool tongueHitCollectible = false;

    public int heldCollectables = 0;

    private LineRenderer line;
    [HideInInspector]
    public GameObject hitCollectable;

    // Start is called before the first frame update
    void Start()
    {
        line = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // If the tongue is not retracting into a wall then roation is allowed
        //if (!tongueHitEnvionment)
        //{
        //    if (Input.GetKey(KeyCode.Q))
        //    {
        //        transform.Rotate(-Vector3.up * rotateSpeed * Time.deltaTime);
        //    }
        //    else if (Input.GetKey(KeyCode.E))
        //    {
        //        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
        //    }
        //}
        // Movement
        //transform.Translate(transform.right * speed * Time.deltaTime * Input.GetAxis("Horizontal"));
        //transform.Translate(transform.forward * speed * Time.deltaTime * Input.GetAxis("Vertical"));

        // Tougue lash
        if (Input.GetKey(KeyCode.Return))
        {
            // The object that the circleCast hits if it hits something
            RaycastHit hit;

            // Casts sphereCast
            if (Physics.SphereCast(transform.position, sphereCastRadus, transform.forward, out hit, Mathf.Infinity))
            {
                tongueHit = hit.point;
                timeLeftToRetract = (timeToGrappleCompletion * 50f/*Frame rate for fixed update*/);
                // If the tongue has hit a wall or other environment
                if (environmentLayer.value  == (1 << hit.collider.gameObject.layer))
                {
                    tongueHitEnvionment = true;
                }
                // If the wall has hit a collectable
                else if (collectableLayer.value == (1 << hit.collider.gameObject.layer))
                {
                    hitCollectable = hit.collider.gameObject;
                    tongueHitCollectible = true;
                }
            }
        }
    }
    void FixedUpdate()
    {
        // If the tongue has hit a wall or other environment
        if (tongueHitEnvionment)
        {
            // Draw tongue to position hit
            if (line != null)
            {
                Vector3[] points = new Vector3[]
                {
                transform.position,
                tongueHit
                };
                line.SetPositions(points);
            }

            // Move player towards tongue
            transform.position += (tongueHit - transform.position) / timeLeftToRetract;
            Vector3 differnce = new Vector3(transform.position.x - tongueHit.x, transform.position.y - tongueHit.y, transform.position.z - tongueHit.z);
            // Check if the tongue has fully retracted
            if (differnce.x < acceptanceRange && differnce.x > -acceptanceRange &&
                differnce.y < acceptanceRange && differnce.y > -acceptanceRange &&
                differnce.z < acceptanceRange && differnce.z > -acceptanceRange)
            {
                tongueHitEnvionment = false;
            }
            // Timer to time to retract
            timeLeftToRetract--;
        }
        // If the tongue has hit a collectable
        else if (tongueHitCollectible)
        {
            // Draw tongue to collectable
            if (line != null)
            {
                Vector3[] points = new Vector3[]
                {
                transform.position,
                hitCollectable.transform.position
                };
                line.SetPositions(points);
            }
            // Move collectable towards player
            hitCollectable.transform.position -= (hitCollectable.transform.position - transform.position) / timeLeftToRetract;

        }
        /*
        // If the tongue isn't connected to anything (Testing only)
        else
        {
            // Draw stationary line
            if (line != null)
            {
                Vector3[] points = new Vector3[]
                {
                transform.position,
                    transform.position + (transform.rotation * transform.forward * 2)
                };
                line.SetPositions(points);
            }
        }
        */
    }
}
