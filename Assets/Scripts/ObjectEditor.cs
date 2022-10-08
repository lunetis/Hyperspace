using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectEditor : MonoBehaviour
{
    // Object creation
    public List<GameObject> creatableObjects;
    public int objectIndex;

    bool isThrow;
    public float throwForce;


    // Raycast components
    public Transform raycastOrigin;

    public MoveScript moveScript;

    int layerMask;

    public GameObject debugObject;
    Material debugMaterial;

    public float raycastDistance;

    void CreateObject()
    {
        if(objectIndex < creatableObjects.Count) return;

        GameObject obj = Instantiate(creatableObjects[objectIndex - 1], debugObject.transform.position, Quaternion.identity);

        if(isThrow == true)
        {
            Rigidbody rigidbody = obj.GetComponent<Rigidbody>();

            if(rigidbody == null) return;

            rigidbody.velocity = raycastOrigin.forward * throwForce;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        layerMask = (1 << LayerMask.NameToLayer("Ground"));
        debugMaterial = debugObject.GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Physics.Raycast(raycastOrigin.position, raycastOrigin.forward, out hit, raycastDistance, layerMask);
        // Debug.Log(moveScript.LookRotation);
        if(hit.collider != null)
        {
            debugObject.transform.position = hit.point;
            debugMaterial.color = Color.blue;
        }
        else
        {
            debugObject.transform.position = raycastOrigin.position + raycastOrigin.forward * raycastDistance;
            debugMaterial.color = Color.red;
        }


        if(Input.GetMouseButtonDown(0))
        {
            CreateObject();
        }

        isThrow = Input.GetKey(KeyCode.LeftShift);
    }
}
