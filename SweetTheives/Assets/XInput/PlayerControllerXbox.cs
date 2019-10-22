using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;


public class PlayerControllerXbox : MonoBehaviour
{
    // What is this player (first, second, ect)
    [SerializeField] XboxController controller = XboxController.All;
    //max move speeds
    [SerializeField] float moveSpeed = 500;
    // Control to fire the tongue
    [SerializeField] XboxButton tongueButton = XboxButton.RightBumper;

    private static bool didQueryNumOfCtrlrs = false;

    // The layer that the walls and other environment are in
    [SerializeField] LayerMask environmentLayer;
    // The layer that the collectables are in
    [SerializeField] LayerMask collectableLayer;
    [SerializeField] GameObject collectablePrefab = null;

    // The max collectables that the player can hold
    public int maxHeldCollectables = 3;
    // The amount that the grapple accelerates object every second
    [SerializeField] float grappleAcceleration = 20;
    // Should the tongue be able to wrap around other environment when already attached
    [SerializeField] bool tongueWrapOn = true;
    // How long you have to wait before you can fire the tongue again
    [SerializeField] public float tongueCooldown;
    // The start draw position for the tongue offset from the player
    [SerializeField] Vector3 tongueOffset = Vector3.zero;
    // How close to the collision position for the tongue it has to be for it to release
    [SerializeField] float acceptanceRange = 0.5f;
    // The radius for the sphere cast
    [SerializeField] float sphereCastRadus = 1f;
    // How many collectables is the player holding
    public int heldCollectables = 0;
    // If the tongue is currently retracting
    private bool tongueHitEnvionment = false;



    /* --Variables needed by the collectableController-- */
    // If the tongue hit a collectable or not
    [HideInInspector] public bool tongueHitCollectible = false;
    // The object that the tongue hit
    [HideInInspector] public GameObject objectHit = null;


    /* The positions that the players tongue is attached to in the environment
            [0] Is the position of the player (where is starts displaying the tongue)
            [1] Is the first position that the tongue is attacked to
            [2]-[?] Is if there is more positions that the tongue is chained to*/
    [HideInInspector] public List<Vector3> tongueHitPoints;

    private Rigidbody rb;
    private LineRenderer line;

    /* The collectable that someone else has stolen from this player 
     * (used to make sure that the player doesn't instantly pick it back up.*/
    [HideInInspector] public GameObject stolenCollectable = null;
    // What the cooldown is currently at
    [HideInInspector] public float currentCooldown = 0.0f;


    // Start is called before the first frame update
    void Start()
    {
        tongueHitPoints = new List<Vector3>();
        tongueHitPoints.Add(Vector3.zero);

        // Only draws tongue if there is a line renderer on the game object
        line = GetComponent<LineRenderer>();
        rb = gameObject.GetComponent<Rigidbody>();
        if (line != null)
        {
            line.enabled = true;
            line.useWorldSpace = true;
        }

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

    /// <summary>
    /// Update loop deals with movement and input
    /// </summary>
    void Update()
    {
        // Get input (put it in x and z because we are moving across those axes)
        Vector3 moveInput = new Vector3(XCI.GetAxisRaw(XboxAxis.LeftStickX, controller), 0.0f, XCI.GetAxisRaw(XboxAxis.LeftStickY, controller));

        // The more your holding the slower you go (0.5 + (amount held / max held) / 2)%
        float speedModifier = ((1 - ((float)heldCollectables / maxHeldCollectables) / 2) + 0.5f);

        // movement from left joystick
        rb.velocity = (moveInput * moveSpeed * speedModifier);
        //rb.AddForce(moveInput * moveSpeed * speedModifier);
        // Look the direction the controller is going if there is input
        Vector3 lookInput = new Vector3(XCI.GetAxisRaw(XboxAxis.RightStickX, controller), 0.0f, XCI.GetAxisRaw(XboxAxis.RightStickY, controller));
        if (lookInput.x != 0 || lookInput.z != 0)
        {
            transform.rotation = Quaternion.LookRotation(lookInput);
        }
        // If there is no input on the look input then face the way the player is moving
        else if (moveInput.x != 0 || moveInput.z != 0)
        {
            transform.rotation = Quaternion.LookRotation(moveInput);
        }

        // Commented out because causing errors because there is no animator on gameObject
        //GetComponent<Animator>().SetFloat("Speed", 1);

        // ---Tongue lash---
        if (currentCooldown >= -0.0001f)
        {
            currentCooldown -= Time.deltaTime;
        }
        if (XCI.GetButtonDown(tongueButton, controller)/*Button is pressed*/  &&
            !tongueHitCollectible && !tongueHitEnvionment /*Tongue is not already connected to something*/ &&
            heldCollectables < maxHeldCollectables /*The player is holding less then the max amount of collectables*/ &&
            currentCooldown <= 0 /*Tongue cooldown is finished*/)
        {
            // Clear points that the tongue

            // Casts sphereCast. hit = The object that the circleCast hits if it hits something
            if (Physics.SphereCast(transform.position, // Start position
                sphereCastRadus, // Width
                transform.forward, // Direction
                out RaycastHit hit, // Output
                Mathf.Infinity)) // Distance
            {
                tongueHitPoints.Clear();
                tongueHitPoints.Add(Vector3.zero);
                tongueHitPoints.Add(hit.point);

                //Debug.Log("SphereCase hit " + hit.collider.gameObject.name);
                // Set what it hit.
                objectHit = hit.collider.gameObject;
                // If the tongue has hit a wall or other environment
                if (environmentLayer.value == (1 << objectHit.layer))
                {
                    // The positions that the tongue hit
                    tongueHitEnvionment = true;
                    tongueHitCollectible = false;
                }
                // If the wall has hit a collectable
                else if (collectableLayer.value == (1 << objectHit.layer))
                {
                    CollectableController collectable = objectHit.GetComponent<CollectableController>();
                    if (collectable.stackSize > 1)
                    {
                        collectable.stackSize--;
                        objectHit = Instantiate(collectablePrefab,
                            objectHit.transform.position,
                            objectHit.transform.rotation);
                    }
                    tongueHitEnvionment = false;
                    tongueHitCollectible = true;
                    objectHit.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
                }
                // If you hit another player
                else if (objectHit.tag == "Player")
                {
                    PlayerControllerXbox player = objectHit.GetComponent<PlayerControllerXbox>();
                    // If the player has something to steal
                    if (player.heldCollectables > 0)
                    {
                        player.heldCollectables--;
                        objectHit = Instantiate(collectablePrefab,
                            objectHit.transform.position,
                            objectHit.transform.rotation);

                        player.stolenCollectable = objectHit;
                        tongueHitCollectible = true;
                    }
                    // If the player has nothing to steal
                    else
                    {
                        tongueHitCollectible = false;
                        objectHit = null;
                    }
                    tongueHitEnvionment = false;
                }
                // If the tongue activated but didn't hit anything
                else
                {
                    objectHit = null;
                    tongueHitCollectible = false;
                    tongueHitEnvionment = false;
                }
                if (objectHit != null)
                {
                    // Set tongue to active
                    line.enabled = true;
                }
            }
        }
    }

    /// <summary>
    /// Fixed update look deals with the tongue actions
    /// </summary>
    void FixedUpdate()
    {
        //rb.velocity = Vector3.zero;

        // Where the start of the tongue is drawn
        tongueHitPoints[0] = transform.TransformPoint(tongueOffset);

        // If the tongue has hit a wall or other environment
        if (tongueHitEnvionment)
        {
            // Draw tongue to position hit
            if (line != null)
            {
                line.positionCount = tongueHitPoints.Count;
                line.SetPositions(tongueHitPoints.ToArray());
            }
            // The separation between the player and the grapple point.
            Vector3 difference = transform.position - tongueHitPoints[1];

            bool shouldAddWrap;
            // Only move to the position that the tongue hit if the button is not pressed down
            if (!XCI.GetButton(tongueButton, controller))
            {
                // Move player towards tongue
                rb.AddForce(-difference.normalized * grappleAcceleration);
                shouldAddWrap = false;
            }
            else
            {
                shouldAddWrap = true;
            }
            if (tongueWrapOn)
            {
                this.CheckTongueWrap(shouldAddWrap);
            }

            // Check if the tongue has fully retracted
            if (difference.x < acceptanceRange && difference.x > -acceptanceRange &&
                difference.y < acceptanceRange && difference.y > -acceptanceRange &&
                difference.z < acceptanceRange && difference.z > -acceptanceRange)
            {
                // If there is more then one point that the tongue is attached to then remove one
                if (tongueHitPoints.Count > 2)
                {
                    tongueHitPoints.RemoveAt(1);
                }
                else
                {
                    tongueHitEnvionment = false;
                    // Set cooldown
                    currentCooldown = tongueCooldown;
                }
            }
        }
        // If the tongue has hit a collectable
        else if (tongueHitCollectible)
        {
            // Draw tongue to collectable
            if (line != null)
            {
                line.positionCount = tongueHitPoints.Count;
                line.SetPositions(tongueHitPoints.ToArray());
            }
            // Only move the collectable to the player if the button is not pressed down
            if (!XCI.GetButton(tongueButton, controller))
            {
                // Move collectable towards player
                Vector3 towardsPlayer = (transform.position - objectHit.transform.position).normalized;

                objectHit.GetComponent<Rigidbody>().AddForce(grappleAcceleration * towardsPlayer);
            }
        }
        else
        {
            if (line != null)
            {
                // Set tongue inactive
                line.enabled = false;
                line.positionCount = 0;
            }
        }
    }

    /// <summary>
    /// Checks if the tongue needs to wrap around an object or should be unwrapped
    /// </summary>
    private void CheckTongueWrap(bool enableAdding = true)
    {
        float acceptanceModifier = 5f;
        RaycastHit hit;

        // If it can't see the next position then add a new one
        if (Physics.Raycast(transform.position, // Start position
            (tongueHitPoints[1] - transform.position).normalized, // Direction
            out hit, // Output
            Vector3.Distance(transform.position, tongueHitPoints[1])
            - acceptanceRange / acceptanceModifier, // Distance
            environmentLayer, // What layers should it collide with 
            QueryTriggerInteraction.UseGlobal))
        {
            if (enableAdding)
            {
                tongueHitPoints.Insert(1, hit.point);
            }
        }

        // If you can see the next point and the one previous then remove the next one
        else if (tongueHitPoints.Count > 2 && !Physics.Raycast(transform.position, // Start position
            (tongueHitPoints[2] - transform.position).normalized, // Direction
            out hit, // Output
            Vector3.Distance(transform.position, tongueHitPoints[2])
            - acceptanceRange / acceptanceModifier,  // Distance
            environmentLayer, // What layers should it collide with 
            QueryTriggerInteraction.UseGlobal))
        {
            tongueHitPoints.RemoveAt(1);
        }
    }
}
