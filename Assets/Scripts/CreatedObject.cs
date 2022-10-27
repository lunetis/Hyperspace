using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CreatedObject : MonoBehaviour
{
    private bool init = false;

    [SerializeField]
    private string ownership;

    public string Ownership
    {
        set
        {
            if(init == true) return;

            init = true;
            ownership = value;
        }

        get { return ownership; }
    }
}
