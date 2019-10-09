using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimerRestartGame : MonoBehaviour
{
    public float timer = 0.0f;
    public float xpos = 0.0f;
    public float ypos = 0.0f;
    public float xsize = 0.0f;
    public float ysize = 0.0f;
    public string text = "";
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            timer = 0;
            //You can restart the scene here
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
    private void OnGUI()
    {
        int minutes = (int)(timer / 60);
        int seconds = (int)timer % 60;
        GUI.Box(new Rect(xpos, xpos, xsize, ysize), text + minutes.ToString() + ":" + seconds.ToString());
    }
}
