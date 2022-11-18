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
    }

    public void SetCamera()
    {
        index = (index + 1) % cameras.Count;
        for(int i = 0; i < cameras.Count; i++)
        {
            cameras[i].gameObject.SetActive(index == i);
        }
    }
}
