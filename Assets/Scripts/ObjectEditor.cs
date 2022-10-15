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

    public GameObject selectedMovableObject;

    public GameObject movingObject;

    int movableObjectLayer;
    int movingObjectLayer;

    bool isMoving;

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

    void MoveObject()
    {
        if(movingObject == null) return;

        RaycastHit hit;
        Physics.Raycast(raycastOrigin.position, raycastOrigin.forward, out hit, raycastDistance, layerMask);

        if(hit.collider != null)
        {
            movingObject.transform.position = hit.point;
        }
        else
        {
            movingObject.transform.position = raycastOrigin.position + raycastOrigin.forward * raycastDistance;
        }

        movingObject.transform.rotation = transform.rotation;
        movingObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }


    // Start is called before the first frame update
    void Start()
    {
        movableObjectLayer = LayerMask.NameToLayer("MovableObject");
        movingObjectLayer = LayerMask.NameToLayer("MovingObject");
        layerMask = (1 << LayerMask.NameToLayer("Ground"));
        layerMask += (1 << movableObjectLayer);
        
        debugMaterial = debugObject.GetComponent<MeshRenderer>().material;

        isMoving = false;
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Physics.Raycast(raycastOrigin.position, raycastOrigin.forward, out hit, raycastDistance, layerMask);
        // Debug.DrawRay(raycastOrigin.position, raycastOrigin.position + raycastOrigin.forward * raycastDistance, Color.red);

        if(hit.collider != null)
        {
            debugObject.transform.position = hit.point;

            if(hit.collider.gameObject.layer == movableObjectLayer)
            {
                debugMaterial.color = Color.green;
                selectedMovableObject = hit.collider.gameObject;
            }
            else
            {
                debugMaterial.color = Color.blue;
                selectedMovableObject = null;
            }
        }
        else
        {
            debugObject.transform.position = raycastOrigin.position + raycastOrigin.forward * raycastDistance;
            debugMaterial.color = Color.red;
            selectedMovableObject = null;
        }


        if(Input.GetMouseButtonDown(0))
        {
            CreateObject();
        }

        if(Input.GetMouseButtonDown(1) && selectedMovableObject != null)
        {
            isMoving = true;
            movingObject = selectedMovableObject;
            movingObject.layer = movingObjectLayer;
        }

        if(Input.GetMouseButtonUp(1) && isMoving == true)
        {
            isMoving = false;
            
            movingObject.layer = movableObjectLayer;
            movingObject = null;
        }

        if(isMoving == true)
        {
            MoveObject();
        }        

        isThrow = Input.GetKey(KeyCode.LeftShift);
    }
}
