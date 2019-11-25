using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPancakeCount : MonoBehaviour
{
    public Image pancakeCount;
    public Image pancakes;
   
    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
                
        PlayerControllerXbox playerscripter = player.GetComponent<PlayerControllerXbox>();
        int maxPancake = playerscripter.maxHeldCollectables;
        int heldPancakeCount = playerscripter.heldCollectables;
    
        pancakeCount.fillAmount = heldPancakeCount / (float)maxPancake;
      
    }
}
