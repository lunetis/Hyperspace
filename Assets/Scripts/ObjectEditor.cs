using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public float raycastDistance;


    // Object moving
    public GameObject selectedMovableObject;
    public GameObject movingObject;
    bool isMoving;

    // Scroll to rotate moving object
    float objectRotation;
    public float rotationAmount;
    Rigidbody movingObjectRigidbody;

    // Misc.
    int movableObjectLayer;
    int movingObjectLayer;

    
    public GameObject debugObject;
    Material debugMaterial;


    void CreateObject()
    {
        if(objectIndex >= creatableObjects.Count || objectIndex < 0) return;
        GameObject obj = Instantiate(creatableObjects[objectIndex], debugObject.transform.position, Quaternion.identity);

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

        // Set Rotation and clear velocity;
        movingObject.transform.eulerAngles = transform.eulerAngles + new Vector3(0, objectRotation, 0);

        if(movingObjectRigidbody != null)
        {
            movingObjectRigidbody.velocity = Vector3.zero;
        }
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

        FindObjectOfType<CreatableObjectButtonUI>()?.SetButton(this, creatableObjects);
    }


    GameObject GetParentObject(GameObject selectedMovableObject)
    {
        Transform current = selectedMovableObject.transform;
        Transform parent = selectedMovableObject.transform.parent;

        while(parent != null && parent.gameObject.layer == movableObjectLayer)
        {
            current = parent;
            parent = current.parent;
        }

        return current.gameObject;
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


        // Mouse input
        // Left Click: Create object
        if(Input.GetMouseButtonDown(0))
        {
            objectRotation = 0;
            CreateObject();
        }

        // Left click release
        if(Input.GetMouseButtonDown(1) && selectedMovableObject != null)
        {
            isMoving = true;

            movingObject = GetParentObject(selectedMovableObject);
            movingObjectRigidbody = movingObject.GetComponent<Rigidbody>();

            // Set layer
            movingObject.layer = movingObjectLayer;
            foreach(Transform child in movingObject.transform)
            {
                child.gameObject.layer = movingObjectLayer;
            }
        }

        // Right Click: Move movable object
        if(Input.GetMouseButtonUp(1) && isMoving == true)
        {
            isMoving = false;
            
            // Set layer
            movingObject.layer = movableObjectLayer;
            foreach(Transform child in movingObject.transform)
            {
                child.gameObject.layer = movableObjectLayer;
            }
            movingObject = null;
        }

        // Right click release: stop moving object
        if(movingObject != null && Input.mouseScrollDelta != Vector2.zero)
        {
            objectRotation += Input.mouseScrollDelta.y * Time.deltaTime * rotationAmount;
        }

        if(isMoving == true)
        {
            MoveObject();
        }

        isThrow = Input.GetKey(KeyCode.LeftShift);
    }
}
