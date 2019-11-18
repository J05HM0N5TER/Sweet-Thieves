/* ---Requirements--- is it plugged in?
 * On player / parent game object:
 * - Line renderer:
 *      ~ Positions - Size = 0
 * - Rigidbody
 * - Capsule collider
 * - Animator:
 *      ~ Controller = ChameleonAnimationController
 *      ~ Avatar = Chameleon_001Avatar
 *  - Tag = Player
 * 
 * On script
 * - Controller
 * - Environment layer
 * - Collectable layer
 * - Collectable prefab
 *  
 * On environment:
 * - Layer = Environment
 * - Collider
 * 
 * On Collectable:
 * - CollectableControler script and specified requirements
 * */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

/// <summary>
/// Used to keep track on what the tongue is interacting with
/// </summary>
public enum HitType : byte
{
	NONE,
	ENVIRONMENT,
	COLLECTABLE
}

[RequireComponent(typeof(Rigidbody), typeof(LineRenderer), typeof(Collider))]
[RequireComponent(typeof(Animation))]
public class PlayerControllerXbox : MonoBehaviour
{
	// What is this player (first, second, ect)
	[SerializeField] XboxController controller = XboxController.All;
	// Max move speeds
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
	[SerializeField] float tripForce = 250;
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

	private Vector3 SpawnPos;
	private Quaternion SpawnRot;

	/* --Variables needed by the collectableController-- */
	// What the kind of interaction does the tongue have with the hit
	[HideInInspector] public HitType tongueHit = HitType.NONE;
	// The object that the tongue hit
	[HideInInspector] public GameObject objectHit = null;


	/* The positions that the players tongue is attached to in the environment
            [0] Is the position of the player (where is starts displaying the tongue)
            [1] Is the first position that the tongue is attacked to
            [2]-[?] Is if there is more positions that the tongue is chained to*/
	[HideInInspector] public List<Vector3> tongueHitPoints;

	private Rigidbody rb;
	private LineRenderer line;

	// What the cooldown is currently at
	[HideInInspector] public float currentCooldown = 0.0f;


	// Initialisation of animation stuff
	private Animator anim;
    // are they holding an amount of pancakes
    public bool onePancake = false;
    public bool twoPancake = false;
    public bool threePancake = false;
    // the hand bone that they pancakes will be childed to
    [SerializeField] Transform  hand = null;
    // what MESH will be spawned, this needs to have NOTHING but a mesh.
    [SerializeField] GameObject pancakeMesh = null;
    // where the pancake will spawn on the character(offset from the hand)
    [HideInInspector] Vector3 holdingPosition = new Vector3(0.221f,-0.318f,0.084f);
    // stuff to turn quaternion to vector3
    private Vector3 holdingRotationEuler = new Vector3(72.284f, 0, 0);
    // how many pancakes are connected to the player currently
    int pancakesspawned = 0;
    // Gameobjects of each pancak connected to the player.
    private GameObject heldpancake1 = null;
    private GameObject heldpancake2 = null;
    private GameObject heldpancake3 = null;

	private float playerHeight;
    //particle system
    [SerializeField] ParticleSystem runparticles = null;

    // Start is called before the first frame update
    void Start()
	{
		playerHeight = GetComponent<CapsuleCollider>().bounds.size.y;

		tongueHitPoints = new List<Vector3>
		{
			Vector3.zero
		};

		// Only draws tongue if there is a line renderer on the game object
		line = GetComponent<LineRenderer>();
		line.enabled = true;
		line.useWorldSpace = true;

		rb = gameObject.GetComponent<Rigidbody>();

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
		// getting component
		anim = GetComponent<Animator>();

		// Set variables on spawn
		SpawnPos = transform.position;
		SpawnRot = transform.rotation;
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

		// ---Tongue lash---
		if (currentCooldown >= -0.0001f)
		{
			currentCooldown -= Time.deltaTime;
		}
		if (XCI.GetAxis(XboxAxis.RightTrigger, controller) > 0 /*Trigger is pressed*/  &&
			tongueHit == HitType.NONE /*Tongue is not already connected to something*/ &&
			heldCollectables < maxHeldCollectables /*The player is holding less then the max amount of collectables*/ &&
			currentCooldown <= 0 /*Tongue cooldown is finished*/)
		{
			TongueLash();
            anim.SetBool("tongueAttack", true);
		}

		// animation stuff
		anim.SetFloat("runningSpeed", rb.velocity.magnitude);
        if(rb.velocity.magnitude >= 0.1f)
        {
            runparticles.enableEmission = true;
    }
        if(rb.velocity.magnitude <= 0.1)
        {
            runparticles.enableEmission = false;
        }
        if(heldCollectables > 0)
        {
            anim.SetBool("holdingPancakes", true);
        }
        else if(heldCollectables <= 0)
        {
            anim.SetBool("holdingPancakes", false);
        }
        // spawn first pancake at position 
        if (heldCollectables == 1)
        {
            onePancake = true;
            if (onePancake && pancakesspawned <= 0)
            {
                pancakesspawned = 1;
                heldpancake1 = Instantiate(pancakeMesh);
                heldpancake1.transform.SetParent(hand, false);
                heldpancake1.transform.localPosition = holdingPosition;
                heldpancake1.transform.localRotation = Quaternion.Euler(holdingRotationEuler);
                onePancake = false;
            }
        }
        // spawn second pancake with offset
        else if (heldCollectables == 2)
        {
            twoPancake = true;
            if (twoPancake && pancakesspawned <= 1)
            {
                pancakesspawned = 2;
                heldpancake2 = Instantiate(pancakeMesh);
                heldpancake2.transform.SetParent(hand, false);
                heldpancake2.transform.localPosition = new Vector3(0.207f, -0.25f, 0.229f);
                heldpancake2.transform.localRotation = Quaternion.Euler(holdingRotationEuler);
                twoPancake = false;
            }
        }
        //spawn thrid pancake with offset
        else if (heldCollectables == 3)
        {
            threePancake = true;
            if (threePancake && pancakesspawned <= 2)
            {
                pancakesspawned = 3;
                heldpancake3 = Instantiate(pancakeMesh);
                heldpancake3.transform.SetParent(hand, false);
                heldpancake3.transform.localPosition =  new Vector3(0.226f, -0.182f, 0.383f);
                heldpancake3.transform.localRotation = Quaternion.Euler(holdingRotationEuler);
                threePancake = false;
            }
        }
        // delete references to objects as they have been lost.
        else if(heldCollectables == 0)
        {
            Destroy(heldpancake1);
            Destroy(heldpancake2);
            Destroy(heldpancake3);
            pancakesspawned = 0;
        }
       

    }

	/// <summary>
	/// Used to calculate what the tongue should be interacting with
	/// </summary>
	private void TongueLash()
	{
		Vector3 pointModifier = new Vector3(0, (playerHeight / 2) + sphereCastRadus, 0);
		// Casts sphereCast. hit = The object that the circleCast hits if it hits something
		if (Physics.CapsuleCast(transform.position + pointModifier, // First point in capsule
			transform.position - pointModifier, // Second point in capsule
			sphereCastRadus,
			transform.forward, // Direction
			out RaycastHit hit, // Output
			300f)) // Distance (If map gets really big then this number might need to be increased)
		{
			// Set defaults
			tongueHit = HitType.NONE;
			tongueHitPoints.Clear();
			tongueHitPoints.Add(Vector3.zero);
			tongueHitPoints.Add(standardisePosition(hit.point));
			// Set what it hit.
			objectHit = hit.collider.gameObject;

			// If the tongue has hit a wall or other environment
			if ((environmentLayer.value & (1 << objectHit.layer)) != 0)
			{
				tongueHit = HitType.ENVIRONMENT;
				// Set cooldown
				currentCooldown = tongueCooldown;

			}
			// If the wall has hit a collectable
			else if ((collectableLayer.value & (1 << objectHit.layer)) != 0)
			{
				CollectableController collectable = objectHit.GetComponent<CollectableController>();
				if (collectable.stackSize > 1)
				{
					collectable.stackSize--;
					objectHit = Instantiate(collectablePrefab,
						objectHit.transform.position,
						objectHit.transform.rotation);
				}
				tongueHit = HitType.COLLECTABLE;
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

					tongueHit = HitType.COLLECTABLE;
				}
			}
			// If the tongue is attached to something then activate it
			line.enabled = tongueHit != HitType.NONE;
		}
	}

	/// <summary>
	/// Fixed update look deals with the tongue actions
	/// </summary>
	void FixedUpdate()
	{
		// Where the start of the tongue is drawn
		tongueHitPoints[0] = transform.TransformPoint(tongueOffset);

		switch (tongueHit)
		{
			// If the tongue isn't attached to anything then don't do anything
			case HitType.NONE:
				// Set tongue inactive
				line.enabled = false;
				line.positionCount = 0;
                anim.SetBool("tongueAttack", false);
                break;
			// If the tongue has hit a wall or other environment 
			case HitType.ENVIRONMENT:
				TongueEnvironmentInteraction();
				CheckIfTripping();
				break;
			// If the tongue has hit a collectable
			case HitType.COLLECTABLE:
				TongueCollectableInteraction();
				CheckIfTripping();
				break;
			default:
				break;
		}
	}

	/// <summary>
	/// Deals with the interaction between the tongue and the environment
	/// </summary>
	private void TongueEnvironmentInteraction()
	{
		// Draw tongue to position hit
		line.positionCount = tongueHitPoints.Count;
		line.SetPositions(tongueHitPoints.ToArray());

		// The separation between the player and the grapple point.
		Vector3 difference = transform.position - tongueHitPoints[1];

		bool shouldAddWrap;
		// Only move to the position that the tongue hit if the button is not pressed down
		if (!(XCI.GetAxis(XboxAxis.RightTrigger, controller) > 0))
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
			CheckTongueWrap(shouldAddWrap);
		}

		// Check if the tongue has fully retracted
		if (difference.sqrMagnitude < acceptanceRange * acceptanceRange)
		{
			// If there is more then one point that the tongue is attached to then remove one
			if (tongueHitPoints.Count > 2)
			{
				tongueHitPoints.RemoveAt(1);
			}
			else
			{
				tongueHit = HitType.NONE;
			}
		}
	}

	/// <summary>
	/// Deals with the interaction between the tongue and the collectable
	/// </summary>
	private void TongueCollectableInteraction()
	{
		// Draw tongue to collectable
		line.positionCount = tongueHitPoints.Count;
		line.SetPositions(tongueHitPoints.ToArray());

		// Only move the collectable to the player if the button is not pressed down
		if (!XCI.GetButton(tongueButton, controller))
		{
			// Move collectable towards player
			Vector3 towardsPlayer = (transform.position - objectHit.transform.position).normalized;

			objectHit.GetComponent<Rigidbody>().AddForce(grappleAcceleration * towardsPlayer);
		}
	}

	/// <summary>
	/// Checks if the tongue needs to wrap around an object or should be unwrapped
	/// </summary>
	/// <param name="enableAdding">Should it be adding more points for the tongue to wrap around?</param>
	private void CheckTongueWrap(bool enableAdding = true)
	{
		float acceptanceModifier = 2f;
		
		Vector3 pointModifier = new Vector3(0, (playerHeight / 2) + sphereCastRadus, 0);
		Vector3 firstRotation = (tongueHitPoints[1] - transform.position).normalized;

		RaycastHit hit;

		// Check if the tongue should be wrapping around something
		bool canUnwrap = (tongueHitPoints.Count <= 2) ? false : // Automatically false if there is only 2 point in the tongue
			!Physics.Raycast(transform.position, // Start position
			(tongueHitPoints[2] - transform.position).normalized, // Direction
			out hit, // Output (assigned but ignored)
			Vector3.Distance(transform.position, tongueHitPoints[2])
			- acceptanceRange / acceptanceModifier,  // Distance
			environmentLayer); // What layers should it collide with

		/* If the player can't directly see the next point for the tongue then
		 * something is in the way and the tongue needs to wrap around it */
		bool needWrap = Physics.CapsuleCast(transform.position + pointModifier, // First point in capsule
			transform.position - pointModifier, // Second point in capsule
			acceptanceRange,
			firstRotation, // Direction
			out hit, // Output
			Vector3.Distance(transform.position, tongueHitPoints[1])
			- (acceptanceRange * acceptanceModifier) - sphereCastRadus, // Distance
			environmentLayer); // What layers should it collide with


		// If it can't see the next position then add a new one
		if (needWrap && (!canUnwrap && tongueHitPoints.Count > 2))// What layers should it collide with 
		{
			if (enableAdding)
			{
				tongueHitPoints.Insert(1, standardisePosition(hit.point));
			}
		}

		// If you can see the next point and the one previous then remove the next one
		else if (tongueHitPoints.Count > 2 && canUnwrap && !needWrap) 
		{
			tongueHitPoints.RemoveAt(1);
		}
	}

	/// <summary>
	/// Gets the base that the player is connected to
	/// </summary>
	/// <returns>The GameObject of the base</returns>
	public GameObject GetBase()
	{
		foreach (GameObject current in GameObject.FindGameObjectsWithTag("Player Base"))
		{
			if (current.GetComponent<BaseStash>().player.transform == transform)
			{
				return current;
			}
		}
		return null;
	}

	/// <summary>
	/// Checks if any of the tongue for this player is tripping another player
	/// </summary>
	private void CheckIfTripping()
	{
		for (int i = 0; i < tongueHitPoints.Count - 1; i++)
		{
			if (Physics.Linecast(tongueHitPoints[i], tongueHitPoints[i + 1], out RaycastHit hit) // If the linecast has hit something
				&& hit.collider.tag == "Player" // The linecast hit a player
				&& hit.collider.gameObject != gameObject) // And it is not the player that the tongue is from
			{
				hit.collider.GetComponent<PlayerControllerXbox>().TripPlayer();
			}
		}
	}

	/// <summary>
	/// For when the player is tripped by the tongue of another player
	/// </summary>
	public void TripPlayer()
	{
		DropCollectables();

		rb.AddForce(transform.forward * tripForce, ForceMode.Impulse);
	}

	/// <summary>
	/// Drops all the collectibles that the player has on hand in the current position stacked.
	/// </summary>
	public void DropCollectables()
	{
		if (heldCollectables == 0)
		{
			return;
		}
		else if (heldCollectables == 1)
		{
			Instantiate(collectablePrefab, transform.position, transform.rotation);
		}
		else
		{
			// Get the collectable height to stack on spawn
			float collectableHeight = collectablePrefab.GetComponent<CapsuleCollider>().bounds.size.y + 0.05f;

			for (int i = 0; i < heldCollectables; i++)
			{
				Instantiate(collectablePrefab, new Vector3(gameObject.transform.position.x, i * collectableHeight, gameObject.transform.position.z), transform.rotation);
			}
		}

		// Reset held collectables
		heldCollectables = 0;
	}

	/// <summary>
	/// For when the player needs to be sent back to spawn
	/// </summary>
	public void ResetToSpawn()
	{
		transform.position = SpawnPos;
		transform.rotation = SpawnRot;
	}

	/// <summary>
	/// Sets the Y coordinate to the same as the player
	/// </summary>
	/// <param name="newPosition">The vector that is being edited</param>
	/// <returns>The input vector with the Y coordinate edited</returns>
	private Vector3 standardisePosition(Vector3 newPosition)
	{
		return new Vector3(newPosition.x, transform.position.y, newPosition.z);
	}
}