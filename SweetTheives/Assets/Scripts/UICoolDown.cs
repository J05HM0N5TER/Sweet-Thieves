using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UICoolDown : MonoBehaviour
{
    //this is the image of the bar that is filling up 
    public Image coolDownImage;

    public GameObject player;
  
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        PlayerControllerXbox playerscript = player.GetComponent<PlayerControllerXbox>();//max
        float maxCoolDownTime  = playerscript.tongueCooldown;
        float currentCoolDownTime = playerscript.currentCooldown;
        
        if (currentCoolDownTime > 0f)
        {
            coolDownImage.fillAmount = 1 - (currentCoolDownTime / maxCoolDownTime);
        }
        else
        {
            coolDownImage.fillAmount = 1;
        }
    }
}
