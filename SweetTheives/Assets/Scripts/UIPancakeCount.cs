using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPancakeCount : MonoBehaviour
{
    public Image pancakeCount;
    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //this is very broken dont worry about this yet
        PlayerControllerXbox playerscripter = player.GetComponent<PlayerControllerXbox>();
        int maxPancake = playerscripter.maxHeldCollectables;
        int currentPancake = playerscripter.heldCollectables;
         
        if(currentPancake > 0)
        {
            
            pancakeCount.fillAmount = 1 - (currentPancake / maxPancake);
        }
        else
        {
            pancakeCount.fillAmount = 1;
        }
    }
}
