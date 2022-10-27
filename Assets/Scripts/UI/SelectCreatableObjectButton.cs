using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectCreatableObjectButton : MonoBehaviour
{
    public GameObject creatableObject;
    public int index;

    public ObjectEditor objectEditor;


    void SetData(GameObject creatableObject, int index)
    {
        this.creatableObject = creatableObject;
        this.index = index;
    }
}
