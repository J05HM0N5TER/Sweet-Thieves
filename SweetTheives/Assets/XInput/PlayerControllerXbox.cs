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

public enum PlayerState : byte
{
	NORMAL,
	TRIPPED,
	RETRACTING
}

[RequireComponent(typeof(Rigidbody), typeof(LineRenderer), typeof(Collider))]
[RequireComponent(typeof(Animation))]
[RequireComponent(typeof(AudioSource))]
public class PlayerControllerXbox : MonoBehaviour
{
#pragma warning disable IDE0044 // Add readonly modifier
	//the order the winner for UI appear starts at 0
	public int PortraitIndex;
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
	// How far the player gets thrown when tripped
	[SerializeField] float tripForce = 250;
	// How long does the player get stunned for when tripped
	[SerializeField] float StunnedTime;
	// Should the tongue be able to wrap around other environment when already attached
	[SerializeField] bool tongueWrapOn = true;
	// How many new points it adds with the tongue wrap
	[SerializeField] float tongueWrapSensitivity = 1f;
	// How long you have to wait before you can fire the tongue again
	public float tongueCooldown;
	[SerializeField] float maxGrappleTime = 6f;
	// The start draw position for the tongue offset from the player
	[SerializeField] Vector3 tongueOffset = Vector3.zero;
	// How close to the collision position for the tongue it has to be for it to release
	[SerializeField] float tongueReleaseRange = 1.5f;
	// The radius for the sphere cast
	[SerializeField] float tongueFireRadius = 0.1f;
	// How many collectables is the player holding
	public int heldCollectables = 0;

	private Vector3 SpawnPos;
	private Quaternion SpawnRot;

	/* --Variables needed by the collectableController-- */
	// What the kind of interaction does the tongue have with the hit
	[HideInInspector] public HitType tongueHit = HitType.NONE;
	// The object that the tongue hit
	public GameObject objectHit = null;


	/* The positions that the players tongue is attached to in the environment
            [0] Is the position of the player (where is starts displaying the tongue)
            [1] Is the first position that the tongue is attacked to
            [2]-[?] Is if there is more positions that the tongue is chained to*/
	[HideInInspector] public List<Vector3> tongueHitPoints;

	private Rigidbody rb;
	private LineRenderer line;

	// What the cooldown is currently at
	[HideInInspector] public float currentCooldown = 0.0f;
	private float currentGrappleTime = 0.0f;
	private float currentStunnedTime = 0.0f;


	// Initialisation of animation stuff
	private Animator anim;
	// the hand bone that they pancakes will be childed to
	[SerializeField] Transform hand = null;
	/* what MESH will be spawned, this needs to have a game object 
		  with a mesh filter and renderer and nothing else.*/
	[SerializeField] GameObject displayCollectableMesh = null;
	// All of the display collectables
	private List<GameObject> displayCollectables = new List<GameObject>();
	// The height of the display collectables mesh (used to stack correctly)
	private float displayCollectableHeight;
	// where the pancake will spawn on the character(offset from the hand)
	[HideInInspector] Vector3 holdingPosition = new Vector3(0.221f, -0.318f, 0.084f);// = new Vector3(0.221f, -0.318f, 0.084f);

	private float playerHeight;
	//particle system
	[SerializeField] ParticleSystem runparticles = null;
	[SerializeField] ParticleSystem tripParticles = null;

	//sound stuff
	AudioSource audiosource;
	[HideInInspector] public bool tripping = false;
	// What state the player is currently in
	private PlayerState playerState = PlayerState.NORMAL;
	//sound for when the player is tripped
	[SerializeField] AudioClip Fall = null;
	// audio and bool to play sound when tongue cooldown is finished
	[SerializeField] AudioClip TongueCoolDownFinished = null;
	private bool TongueCoolDownSoundPlayed = false;
	//audio for when the a pancake is being picked up.
	[SerializeField] AudioClip PancakePickUp = null;
	public float vibrationTime = 0.4f;
	[SerializeField] AudioClip venusChomp = null;
	[SerializeField] AudioClip bellSound = null;
	[SerializeField] AudioClip stolenSound = null;

	[HideInInspector] public XInputDotNetPure.PlayerIndex playernumber = 0;
	public float chompDelay = 1.0f;
#pragma warning restore IDE0044 // Add readonly modifier

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
		line.positionCount = 0; ;

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
		// getting component for animation and sound
		anim = GetComponent<Animator>();
		audiosource = GetComponent<AudioSource>();
		TongueCoolDownSoundPlayed = true;
		// Set variables on spawn point
		SpawnPos = transform.position;
		SpawnRot = transform.rotation;

		playernumber = (XInputDotNetPure.PlayerIndex)controller - 1;
	}

	/// <summary>
	/// Update loop deals with movement and input
	/// </summary>
	void Update()
	{
        currentCooldown -= Time.deltaTime;
        // Get input (put it in x and z because we are moving across those axes)
        Vector3 moveInput = new Vector3(XCI.GetAxisRaw(XboxAxis.LeftStickX, controller), 0.0f, XCI.GetAxisRaw(XboxAxis.LeftStickY, controller));

		// Set the speed of the animation based on the speed the player is moving
		anim.SetFloat("runningSpeed", Mathf.Max(Mathf.Abs(moveInput.x), Mathf.Abs(moveInput.z)));

		if (playerState != PlayerState.TRIPPED)
		{
			// The more your holding the slower you go (speed goes between 0.5 and 1 (the more you have the slower you go)
			float speedModifier = 1 - (heldCollectables / maxHeldCollectables / 2);

			// movement from left joystick
			rb.velocity = (moveInput * moveSpeed * speedModifier);

			// Look the direction the controller is going if there is input
			Vector3 lookInput = new Vector3(XCI.GetAxisRaw(XboxAxis.RightStickX, controller), 0.0f, XCI.GetAxisRaw(XboxAxis.RightStickY, controller));
			if (lookInput != Vector3.zero)
			{
				transform.rotation = Quaternion.LookRotation(lookInput);
			}
			// If there is no input on the look input then face the way the player is moving
			else if (moveInput != Vector3.zero)
			{
				transform.rotation = Quaternion.LookRotation(moveInput);
			}

			// If the player pressed down the button to fire the tongue
			if (XCI.GetAxis(XboxAxis.RightTrigger, controller) > 0 /*Trigger is pressed*/)
			{
                
                // If they are allowed to shoot the tongue
                if (tongueHit == HitType.NONE /*Tongue is not already connected to something*/ &&
				heldCollectables < maxHeldCollectables /*The player is holding less then the max amount of collectables*/ &&
				currentCooldown <= 0 /*Tongue cooldown is finished*/)
				{
					TongueLash();
					anim.SetBool("tongueAttack", true);
				}
				// If they tried to fire the tongue but wasn't allowed
				else
				{
					StartCoroutine(Vibrate());
				}
			}
		}
		// If they are tripped vibrate the controller
		else
		{
			rb.velocity = Vector3.zero;
			StartCoroutine(Vibrate());
		}
		// If the cooldown if finished then play the indicator sound
		if (currentCooldown <= 0 && TongueCoolDownSoundPlayed == false)
		{
			audiosource.PlayOneShot(TongueCoolDownFinished, 1.0f);
			TongueCoolDownSoundPlayed = true;
		}

		// animation stuff
		anim.SetFloat("runningSpeed", rb.velocity.magnitude);

		// If they are holding collectables then show that animation
		if (heldCollectables > 0)
		{
			anim.SetBool("holdingPancakes", true);
		}
		else
		{
			anim.SetBool("holdingPancakes", false);
		}

		// Add any display collectables if not enough
		for (int i = displayCollectables.Count; i < heldCollectables; i++)
		{
			// Add new display collectable
			displayCollectables.Add(Instantiate(displayCollectableMesh));
			// Child to the had so it looks like the player is holding it
			displayCollectables[i].transform.SetParent(hand, false);
			// For calculating the position on stack to display
			Vector3 newPosition = holdingPosition;
			// Add height for stacking to z position because of weird rotation of hand
			newPosition.z += (displayCollectableHeight + 0.05f) * i;
			// Set the new position
			displayCollectables[i].transform.localPosition = newPosition;
			// Compensate for weird hand rotation
			displayCollectables[i].transform.localRotation = Quaternion.Euler(new Vector3(70, 0, 0));
		}

		// Remove any display collectables if too many
		for (int i = displayCollectables.Count; i > heldCollectables; i--)
		{
			GameObject temp = displayCollectables[i - 1];
			// Remove from array
			displayCollectables.Remove(temp);
			// Remove from scene
			Destroy(temp);
		}

		// Play the particles for running if the player is moving
		if (rb.velocity.magnitude > 0.1f)
		{
			runparticles.Play();
		}
		else
		{
			runparticles.Stop();
		}
	}

	/// <summary>
	/// Used to calculate what the tongue should be interacting with
	/// </summary>
	private void TongueLash()
	{
		// Used to calculate the Capsule for the CapsuleCast
		Vector3 pointModifier = new Vector3(0, (playerHeight / 2) + tongueFireRadius, 0);
		// Casts sphereCast. hit = The object that the circleCast hits if it hits something
		if (Physics.CapsuleCast(transform.position + pointModifier, // First point in capsule
			transform.position - pointModifier, // Second point in capsule
			tongueFireRadius, // Radius of Capsule
			transform.forward, // Direction
			out RaycastHit hit, // Output
			300f)) // Distance (If map gets really big then this number might need to be increased)
		{
			// Set defaults
			currentGrappleTime = 0f;
			// Reset player state and tongue hit
			DisconnectTongue();
			// Remove any existing points in tongue
			tongueHitPoints.Clear();
			// Set the first position to zero because it's calculated in update
			tongueHitPoints.Add(Vector3.zero);
			// Set where the tongue has hit
			tongueHitPoints.Add(StandardisePosition(hit.point));
			// Set what it hit.
			objectHit = hit.collider.gameObject;

			// If the tongue has hit a wall or other environment
			if ((environmentLayer.value & (1 << objectHit.layer)) != 0)
			{
				tongueHit = HitType.ENVIRONMENT;
				// Set cooldown
				currentCooldown = tongueCooldown;

			}
			// If the tongue has hit a collectable
			else if ((collectableLayer.value & (1 << objectHit.layer)) != 0)
			{
				// Get the script from the collectable
				CollectableController collectable = objectHit.GetComponent<CollectableController>();
				// If the collectable is a stack then grab only one
				if (collectable.stackSize > 1)
				{
					collectable.stackSize--;
					objectHit = Instantiate(collectablePrefab,
						objectHit.transform.position,
						objectHit.transform.rotation);
				}
				tongueHit = HitType.COLLECTABLE;
				// Freeze the rotation of the collectable to make pulling more smooth
				objectHit.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
			}
			// If you hit another player
			else if (objectHit.tag == "Player")
			{
				PlayerControllerXbox player = objectHit.GetComponent<PlayerControllerXbox>();
				// If the player has something to steal
				if (player.heldCollectables > 0)
				{
					// Remove one from how many they are holding
					player.heldCollectables--;
					// Create new collectable at position of player
					objectHit = Instantiate(collectablePrefab,
						objectHit.transform.position,
						objectHit.transform.rotation);

					tongueHit = HitType.COLLECTABLE;
					// Vibrate their controller
					player.StartCoroutine(Vibrate());
					// Play sound for when collectable is stolen
					player.PlayStolenSound();
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

		// Interaction for the tongue
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

		// Interaction for what state the player is in
		switch (playerState)
		{
			case PlayerState.NORMAL:
				break;
			// If the player has been tripped
			case PlayerState.TRIPPED:
				// Timer
				currentStunnedTime += Time.deltaTime;
				// Disconnect the tongue
				DisconnectTongue();
				// If the player has been stunned for enough time
				if (currentStunnedTime > StunnedTime)
				{
					// Reset timer
					currentStunnedTime = 0f;
					// Set the player to no longer tripping
					playerState = PlayerState.NORMAL;
				}
				break;
			// If the tongue is being retracted
			case PlayerState.RETRACTING:
				TongueRetracting();
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
		currentGrappleTime += Time.deltaTime;

		// Draw tongue to position hit
		line.positionCount = tongueHitPoints.Count;
		line.SetPositions(tongueHitPoints.ToArray());

		// Only move to the position that the tongue hit if the button is not pressed down
		if (!(XCI.GetAxis(XboxAxis.RightTrigger, controller) > 0))
		{
			playerState = PlayerState.RETRACTING;
		}
		// Deal with tongue wrapping
		if (tongueWrapOn)
		{
			CheckTongueWrap(!(playerState == PlayerState.RETRACTING));
		}
		// Check that the player hasn't had the tongue grappled too long
		if (currentGrappleTime > maxGrappleTime)
		{
			playerState = PlayerState.RETRACTING;
		}
	}

	/// <summary>
	/// Deals with the interaction between the tongue and the collectable
	/// </summary>
	private void TongueCollectableInteraction()
	{
		// Timer
		currentGrappleTime += Time.deltaTime;

		// If the tongue has taken too long to retract
		if (currentGrappleTime > maxGrappleTime)
		{
			// Disconnect the tongue from collectable
			tongueHit = HitType.NONE;
			currentGrappleTime = 0f;
		}

		// Draw tongue to collectable
		line.positionCount = tongueHitPoints.Count;
		line.SetPositions(tongueHitPoints.ToArray());

		// Move collectable towards player
		Vector3 towardsPlayer = (transform.position - objectHit.transform.position).normalized;
		objectHit.GetComponent<Rigidbody>().AddForce(grappleAcceleration * towardsPlayer);
	}

	/// <summary>
	/// Checks if the tongue needs to wrap around an object or should be unwrapped
	/// </summary>
	/// <param name="enableAdding">Should it be adding more points for the tongue to wrap around?</param>
	private void CheckTongueWrap(bool enableAdding = true)
	{
		float lookRadius = 0.1f;
		Vector3 pointModifier = new Vector3(0, (playerHeight / 2) + lookRadius, 0);
		RaycastHit hit;

		// If it can't see the next position then add a new one
		if (Physics.CapsuleCast(
			transform.position + pointModifier, // First point in capsule
			transform.position - pointModifier, // Second point in capsule
			lookRadius,
			(tongueHitPoints[1] - transform.position).normalized, // Direction
			out hit, // Output
			Vector3.Distance(transform.position, tongueHitPoints[1])
			- tongueWrapSensitivity, // Distance
			environmentLayer)) // What layers should it collide with)
		{
			if (enableAdding)
			{
				tongueHitPoints.Insert(1, StandardisePosition(hit.point));
			}
		}
		// If you can see the next point and the one previous then remove the next one
		else if (tongueHitPoints.Count > 2 && !Physics.CapsuleCast(
			transform.position + pointModifier, // First point in capsule
			transform.position - pointModifier, // Second point in capsule
			lookRadius,
			(tongueHitPoints[2] - transform.position).normalized, // Direction
			out hit, // Output
			Vector3.Distance(transform.position, tongueHitPoints[2])
			- tongueWrapSensitivity, // Distance
			environmentLayer))
		{
			tongueHitPoints.RemoveAt(1);
		}
	}

	/// <summary>
	/// For when the tongue is pulling the player
	/// </summary>
	private void TongueRetracting()
	{
		// Timer
		currentGrappleTime += Time.deltaTime;

		// The separation between the player and the grapple point.
		Vector3 difference = transform.position - tongueHitPoints[1];

		// Move player towards tongue
		rb.AddForce(-difference.normalized * grappleAcceleration);

		// Check if the tongue has fully retracted or overwrite if tongue is attached too long
		if (difference.sqrMagnitude < tongueReleaseRange * tongueReleaseRange || currentGrappleTime > maxGrappleTime * 2)
		{
			// If there is more then one point that the tongue is attached to then remove one
			if (tongueHitPoints.Count > 2)
			{
				tongueHitPoints.RemoveAt(1);
			}
			// If there is only one point in the tongue then disconnect form environment
			else
			{
				DisconnectTongue();
			}
		}
	}

	/// <summary>
	/// Checks if any of the tongue for this player is tripping another player
	/// </summary>
	private void CheckIfTripping()
	{
		// For every point in the tongue
		for (int i = 0; i < tongueHitPoints.Count - 1; i++)
		{
			// Check if there is any player in the way to the next point in the tongue
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
		DisconnectTongue();
		tripping = true;

		rb.AddForce(transform.forward * tripForce, ForceMode.Impulse);
		tripping = false;

		// Reset the timer for how long they have been stunned for
		currentStunnedTime = 0f;

		// Set the player to tripped
		playerState = PlayerState.TRIPPED;
		// Play the dirt particles for the trip
		StartCoroutine(PlayTripParticles());
		// Play sound
		audiosource.PlayOneShot(Fall, 1.0f);
		// Play animation
		StartCoroutine(PlayTripPlayerAnim());

	}

	/// <summary>
	/// Drops all the collectibles that the player has on hand in the current position stacked.
	/// </summary>
	public void DropCollectables()
	{
		// If the player isn't holding anything then there is nothing to drop
		if (heldCollectables == 0)
		{
			return;
		}
		// If there is one then drop it
		else if (heldCollectables == 1)
		{
			Instantiate(collectablePrefab, transform.position, transform.rotation);
		}
		// If there is more then one then stack when dropped
		else
		{
			// Get the collectable height to stack on spawn
			float collectableHeight = collectablePrefab.GetComponent<CapsuleCollider>().bounds.size.y + 0.05f;

			// For every collectable
			for (int i = 0; i < heldCollectables; i++)
			{
				// Spawn and stack
				Instantiate(collectablePrefab, new Vector3(gameObject.transform.position.x, i * collectableHeight,
					gameObject.transform.position.z), transform.rotation);
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
		// Disconnect tongue to that doesn't have problems with tongue stuck on other side of map
		DisconnectTongue();
	}

	/// <summary>
	/// Sets the Y coordinate to the same as the height that the tongue is fired at
	/// </summary>
	/// <param name="newPosition">The vector that is being edited</param>
	/// <returns>The input vector with the Y coordinate edited</returns>
	private Vector3 StandardisePosition(Vector3 newPosition)
	{
		return new Vector3(newPosition.x, transform.position.y + tongueOffset.y, newPosition.z);
	}

	/// <summary>
	/// For if the tongue is being disconnected from an object
	/// </summary>
	private void DisconnectTongue()
	{
		// Set tongue interaction to none to specify the tongue isn't connected to anything
		tongueHit = HitType.NONE;
		// If the player was retracting tongue then they no longer are
		playerState = (playerState == PlayerState.RETRACTING) ? PlayerState.NORMAL : playerState;
		currentGrappleTime = 0f;
	}

	/// <summary>
	/// Plays the dirt particle effect for when the player trips over
	/// </summary>
	/// <returns>Used for the WaitForSeconds function</returns>
	public IEnumerator PlayTripParticles()
	{
		tripParticles.time = 0f;
		tripParticles.Play();
		yield return new WaitForSeconds(2);
		tripParticles.Stop();
	}

	/// <summary>
	/// Plays the sound for when the player picks up a collectible
	/// </summary>
	public void PlayPickUpSound()
	{
		audiosource.PlayOneShot(PancakePickUp, 1.0f);
	}

	/// <summary>
	/// Vibrates the player
	/// </summary>
	/// <returns>Used for the WaitForSeconds function</returns>
	public IEnumerator Vibrate()
	{
		XInputDotNetPure.GamePad.SetVibration(playernumber, 0.5f, 0.5f);
		yield return new WaitForSeconds(vibrationTime);
		XInputDotNetPure.GamePad.SetVibration(playernumber, 0f, 0f);
		anim.SetBool("tripped", false);
	}

	/// <summary>
	/// Plays the animation for the Trap 
	/// </summary>
	/// <returns>Used for the WaitForSeconds function</returns>
	public IEnumerator VenusChomp()
	{
		yield return new WaitForSeconds(chompDelay);
		audiosource.PlayOneShot(venusChomp, 1.0f);
	}

	/// <summary>
	/// Plays the sound if for a pancake being stolen
	/// </summary>
	public void PlayStolenSound()
	{
		audiosource.PlayOneShot(stolenSound, 1.0f);
	}

	//private bool bellsoundplayed = false;
	/// <summary>
	/// Plays the sound for the bell being hit
	/// </summary>
	public void PlayBellSound()
	{
		audiosource.PlayOneShot(bellSound, 1.0f);
	}

	/// <summary>
	/// Plays the animation for the player being tripped
	/// </summary>
	/// <returns>Used for the WaitForSeconds function</returns>
	public IEnumerator PlayTripPlayerAnim()
	{
		anim.SetLayerWeight(1, 0);
		anim.SetBool("tripped", true);
		yield return new WaitForSeconds(1.0f);
		anim.SetBool("tripped", false);
		anim.SetLayerWeight(1, 1);
	}
}