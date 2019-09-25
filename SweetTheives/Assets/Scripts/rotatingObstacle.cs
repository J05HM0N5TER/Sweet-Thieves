using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotatingObstacle : MonoBehaviour
{
    public int orbitSpeed; 


    void Update()
    {
        orbiting(); 
    }

    // Update is called once per frame
    void orbiting()
    {
        transform.Rotate(Vector3.up * orbitSpeed * Time.deltaTime);
    }
}
