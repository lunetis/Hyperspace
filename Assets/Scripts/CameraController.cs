using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public List<Camera> cameras;
    int index;

    // Start is called before the first frame update
    void Start()
    {
        index = 0;
        SetCamera();
    }

    void SetCamera()
    {
        for(int i = 0; i < cameras.Count; i++)
        {
            cameras[i].gameObject.SetActive(index == i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Camera"))
        {
            index = (index + 1) % cameras.Count;
            SetCamera();
        }
    }
}
