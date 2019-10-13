using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;


public class PlayerControllerXbox : MonoBehaviour
{
    // What is this player (first, second, ect)
    [SerializeField] XboxController controller;
    //max move speeds
    [SerializeField] float maxMoveSpeed = 20;
    // v DEBUG v
    [SerializeField] float dashSpeed = 20;
    [SerializeField] XboxButton dashButton = XboxButton.A;
    // ^ DEBUG ^
    [SerializeField] XboxButton tongueButton = XboxButton.B;

    //public Material matRed;
    //public Material matGreen;
    //public Material matBlue;
    //public Material matYellow;

    private static bool didQueryNumOfCtrlrs = false;

    // The layer that the walls and other environment are in
    [SerializeField] LayerMask environmentLayer;
    // The layer that the collectabels are in
    [SerializeField] LayerMask collectableLayer;
    [SerializeField] GameObject collectablePrefab;

    // The max collectables that the player can hold
    public int maxHeldCollectables = 3;
    // The amount that the grapple accelerates object every second
    [SerializeField] float grappleAcceleration = 20;
    // How long you have to wait before you can fire the tongue again
    [SerializeField] public float tongueCooldown;
    // The start draw position for the tongue offset from the player
    [SerializeField] Vector3 tongueOffset = new Vector3(0, 4.5f, 0);
    // How close to the collision position for the tongue it has to be for it to release
    [SerializeField] float acceptanceRange = 0.5f;
    // The radus for the sphere cast
    [SerializeField] float sphereCastRadus = 5f;
    // How many collectables id the player holding
    public int heldCollectables = 0;
    // If the tongue is currently retracting
    private bool tongueHitEnvionment = false;



    /* --Variables needed by the collectableController-- */
    // If the tongue hit a collectable or not
    [HideInInspector] public bool tongueHitCollectible = false;
    // The object that the tongue hit
    [HideInInspector] public GameObject objectHit;
    // The postion for the hit on the tongue on a wall
    [HideInInspector] public Vector3 hitPoint;

    private Rigidbody rb;
    private LineRenderer line;

    //The current speed of the collectable
    private float collectableSpeed;
    /* The collectable that someone else has stolen from this player 
     * (used to make sure that the player doesn't instantly pick it back up.*/
    [HideInInspector] public GameObject stolenCollectable;
    // What the cooldown is currently at
    [HideInInspector] public float currentCooldown = 0.0f;


    // Start is called before the first frame update
    void Start()
    {
        // Only draws tongue if there is a line renderer on the game object
        line = GetComponent<LineRenderer>();
        rb = gameObject.GetComponent<Rigidbody>();


        //switch (controller)
        //{
        //    case XboxController.First: GetComponent<Renderer>().material = matRed; break;
        //    case XboxController.Second: GetComponent<Renderer>().material = matGreen; break;
        //    case XboxController.Third: GetComponent<Renderer>().material = matBlue; break;
        //    case XboxController.Fourth: GetComponent<Renderer>().material = matYellow; break;
        //}

        if (!didQueryNumOfCtrlrs)
        {
            didQueryNumOfCtrlrs = true;

            int queriedNumberOfCtrlrs = XCI.GetNumPluggedCtrlrs();
            if (queriedNumberOfCtrlrs == 1)
            {
                Debug.Log("Only " + queriedNumberOfCtrlrs + " Xbox controller plugged in.");
            }
            else if (queriedNumberOfCtrlrs == 0)
            {
                Debug.Log("No Xbox controllers plugged in!");
            }
            else
            {
                Debug.Log(queriedNumberOfCtrlrs + " Xbox controllers plugged in.");
            }

            XCI.DEBUG_LogControllerNames();
        }

    }

    // Update is called once per frame
    void Update()
    {
        // Get input (put it in x and z because we are moving accross those axes)
        Vector3 xboxInput = new Vector3(XCI.GetAxis(XboxAxis.LeftStickX, controller), 0.0f, XCI.GetAxis(XboxAxis.LeftStickY, controller));

        // dash FOR DEBUGGING ONLY
        if (XCI.GetButton(dashButton, controller))
        {
            transform.position += xboxInput * dashSpeed * Time.deltaTime;
        }

        // The more your holding the slower you go (0.5 + (amount held / max held) / 2)%
        float speedModifier = (1 - (((float)heldCollectables / maxHeldCollectables) / 2) + 0.5f);

        // movement from left joystick
        transform.position += xboxInput * maxMoveSpeed * Time.deltaTime * speedModifier;

        // Look the direction the controler is going
        if (xboxInput.x != 0 || xboxInput.z != 0)
        {
            transform.rotation = Quaternion.LookRotation(xboxInput);
        }
        // Commented out because causeing errors because there is no animator on gameObject
        //GetComponent<Animator>().SetFloat("Speed", 1);

        // ---Tougue lash---
        if (currentCooldown >= -0.0001f)
        {
            currentCooldown -= Time.deltaTime;
        }
        if (XCI.GetButtonDown(tongueButton, controller)/*Button is pressed*/ &&
            !tongueHitCollectible && !tongueHitEnvionment /*Tongue is not already connected to something*/ &&
            heldCollectables < maxHeldCollectables /*The player is holding less then the max amount of collectables*/ &&
            currentCooldown <= 0 /*Tongue cooldown is finished*/)
        {
            // Casts sphereCast. hit = The object that the circleCast hits if it hits something
            if (Physics.SphereCast(transform.position, sphereCastRadus, transform.forward, out RaycastHit hit, Mathf.Infinity))
            {
                Debug.Log("SphereCase hit " + hit.collider.gameObject.name);
                // Set what it hit.
                objectHit = hit.collider.gameObject;
                // If the tongue has hit a wall or other environment
                if (environmentLayer.value == (1 << objectHit.layer))
                {
                    // The possition that the tongue hit
                    hitPoint = hit.point;
                    tongueHitEnvionment = true;
                    tongueHitCollectible = false;
                }
                // If the wall has hit a collectable
                else if (collectableLayer.value == (1 << objectHit.layer))
                {
                    CollectableController collectablesScript = objectHit.GetComponent<CollectableController>();
                    if (collectablesScript.stackSize > 1)
                    {
                        collectablesScript.stackSize--;
                        objectHit = Instantiate(collectablePrefab, objectHit.transform.position, objectHit.transform.rotation);
                    }
                    hitPoint = hit.point;
                    tongueHitEnvionment = false;
                    tongueHitCollectible = true;
                }
                // If you hit another player
                else if (objectHit.tag == "Player")
                {
                    PlayerControllerXbox player = objectHit.GetComponent<PlayerControllerXbox>();
                    // If the player has something to steal
                    if (player.heldCollectables > 0)
                    {
                        player.heldCollectables--;
                        objectHit = Instantiate(collectablePrefab, objectHit.transform.position, objectHit.transform.rotation);
                        player.stolenCollectable = objectHit;
                        tongueHitCollectible = true;
                    }
                    // If the player has nothing to steal
                    else
                    {
                        tongueHitCollectible = false;
                        objectHit = null;
                    }
                    hitPoint = hit.point;
                    tongueHitEnvionment = false;
                }
                // If the tongue activated but didn't hit anything
                else
                {
                    objectHit = null;
                    tongueHitCollectible = false;
                    tongueHitEnvionment = false;
                }
            }
        }
    }

    void FixedUpdate()
    {
        // Where the start of the tongue is drawn
        Vector3 tongueStartPosition = transform.TransformPoint(tongueOffset);

        // If the tongue has hit a wall or other environment
        if (tongueHitEnvionment)
        {
            // Draw tongue to position hit
            if (line != null)
            {
                line.enabled = true;
                line.SetPositions(new Vector3[]
                    {
                    tongueStartPosition,
                    hitPoint
                });
            }
            // The seperation between the player and the grapple point.
            Vector3 differnce = transform.position - hitPoint;

            // Only move to the position that the tongue hit if the button is not pressed down
            if (!XCI.GetButton(tongueButton, controller))
            {
                // Move player towards tongue
                rb.AddForce(-differnce.normalized * grappleAcceleration, ForceMode.Acceleration);
            }
            // Check if the tongue has fully retracted
            if (differnce.x < acceptanceRange && differnce.x > -acceptanceRange &&
                differnce.y < acceptanceRange && differnce.y > -acceptanceRange &&
                differnce.z < acceptanceRange && differnce.z > -acceptanceRange)
            {
                tongueHitEnvionment = false;
                currentCooldown = tongueCooldown;
            }

        }
        // If the tongue has hit a collectable
        else if (tongueHitCollectible)
        {
            // Draw tongue to collectable
            if (line != null)
            {
                line.enabled = true;
                line.SetPositions(new Vector3[]
                    {
                    tongueStartPosition,
                    objectHit.transform.position
                });
            }
            // Only movve the collectable to the player if the button is not pressed down
            if (!XCI.GetButton(tongueButton, controller))
            {
                // Move collectable towards player
                Vector3 towardsPlayer = (transform.position - objectHit.transform.position).normalized;

                collectableSpeed += grappleAcceleration * Time.fixedDeltaTime;
                objectHit.transform.position = objectHit.transform.position + (towardsPlayer * collectableSpeed) * Time.fixedDeltaTime;
            }
        }
        else
        {
            rb.velocity = Vector3.zero;
            if (line != null)
            {
                line.enabled = false;
                line.SetPositions(new Vector3[]
                    {
                    tongueStartPosition,
                    tongueStartPosition
                });
            }
        }
    }
}
