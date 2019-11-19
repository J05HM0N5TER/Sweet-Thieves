using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]
public class Sound : MonoBehaviour
{
    private PlayerControllerXbox playercontrol;
    private BaseStash basestash;
    private BasePoint basepoint;
    AudioSource audiosource;
    [SerializeField] AudioClip Fall = null;// done
    [SerializeField] AudioClip TongueRetract = null; // if has to be changed
    [SerializeField] AudioClip FiveSecondsLeft = null; // add to timer
    [SerializeField] AudioClip PancakePickUp = null; // if has to be changed to happen only once
    [SerializeField] AudioClip PancakeDrop = null;
    [SerializeField] AudioClip CantUseTongue = null;
    [SerializeField] AudioClip MaxPancakesHeld = null;
    [SerializeField] AudioClip StartGame = null;
    [SerializeField] AudioClip EndGame = null;
    [SerializeField] AudioClip MenuButtonClicked = null;
    [SerializeField] AudioClip MenuScrollthroughButton = null;
    [SerializeField] AudioClip FlytrapChomp = null;
    [SerializeField] AudioClip FlytrapButtonPress = null;
    [SerializeField] AudioClip ThemeMusic = null;
    

    // Start is called before the first frame update
    void Start()
    {
        audiosource = GetComponent<AudioSource>();
        playercontrol = GetComponent<PlayerControllerXbox>();
    }

    // Update is called once per frame
    void Update()
    {
        if(playercontrol.tripping)
        {
            audiosource.PlayOneShot(Fall, 1.0f);
        }
        if(playercontrol.tongueHit != HitType.NONE && playercontrol.tongueCooldown > 0)
        {
            audiosource.PlayOneShot(TongueRetract, 1.0f);
        }
        if(playercontrol.heldCollectables == 1 || playercontrol.heldCollectables == 2 || playercontrol.heldCollectables == 3)
        {
            audiosource.PlayOneShot(PancakePickUp, 1.0f);
        }
        
    }
}
