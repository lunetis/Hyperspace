using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCameraScript : MonoBehaviour
{
    public float degreePerSecond;
    void Start()
    {
        
    }
    
    private void Update()
    {
        transform.Rotate(Vector3.up * Time.deltaTime * degreePerSecond);
    }
}
