using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Photon.Pun;
using System;

public class ObjectEditor : MonoBehaviour
{
    public string ownershipName;

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
    [HideInInspector]
    public GameObject selectedMovableObject;
    [HideInInspector]
    public GameObject movingObject;
    bool isMoving;

    // Scroll to rotate moving object
    float objectRotation;
    public float rotationAmount;
    Rigidbody movingObjectRigidbody;

    // Misc.
    int movableObjectLayer;
    int movingObjectLayer;

    CreatedObject pointingObjectScript;
    
    public GameObject debugObject;
    Material debugMaterial;

    public PhotonView pv;


    void CreateObject()
    {
        if(objectIndex >= creatableObjects.Count || objectIndex < 0) return;
        // GameObject obj = Instantiate(creatableObjects[objectIndex], debugObject.transform.position, Quaternion.identity);

        GameObject obj = PhotonNetwork.Instantiate(
            Path.Combine("PhotonPrefabs", creatableObjects[objectIndex].name),
            debugObject.transform.position, Quaternion.identity);

        if(obj == null)
        {
            Debug.LogWarning("Object not created!");
            return;
        }

        if(isThrow == true)
        {
            Rigidbody rigidbody = obj.GetComponent<Rigidbody>();
            if(rigidbody == null) return;

            rigidbody.velocity = raycastOrigin.forward * throwForce;
        }

        // Add ownership
        CreatedObject createdObject = obj.AddComponent<CreatedObject>();
        createdObject.Ownership = ownershipName;
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


    // Start is called before the first frame update
    void Start()
    {
        if(pv.IsMine == false) return;

        movableObjectLayer = LayerMask.NameToLayer("MovableObject");
        movingObjectLayer = LayerMask.NameToLayer("MovingObject");
        layerMask = (1 << LayerMask.NameToLayer("Ground"));
        layerMask += (1 << movableObjectLayer);
        
        debugMaterial = debugObject.GetComponent<MeshRenderer>().material;

        isMoving = false;
        pointingObjectScript = null;

        FindObjectOfType<CreatableObjectButtonUI>()?.SetButton(this, creatableObjects);
        ownershipName = pv.InstantiationId.ToString();
    }

    void CheckObject()
    {
        RaycastHit hit;
        GameObject currentPointingObject = null;
        Physics.Raycast(raycastOrigin.position, raycastOrigin.forward, out hit, raycastDistance, layerMask);
        // Debug.DrawRay(raycastOrigin.position, raycastOrigin.position + raycastOrigin.forward * raycastDistance, Color.red);

        if(hit.collider != null)
        {
            debugObject.transform.position = hit.point;

            if(hit.collider.gameObject.layer == movableObjectLayer)
            {
                currentPointingObject = GetParentObject(hit.collider.gameObject);
            }
            else
            {
                debugMaterial.color = Color.blue;
                currentPointingObject = null;
            }
        }
        else
        {
            // NullReferenceException occurs when the player exits
            try
            {
                debugObject.transform.position = raycastOrigin.position + raycastOrigin.forward * raycastDistance;
                debugMaterial.color = Color.red;
                currentPointingObject = null;
            }
            catch(NullReferenceException e)
            {
                PhotonNetwork.Destroy(pv);
            }
        }

        // Need to refresh
        if(selectedMovableObject != currentPointingObject)
        {
            // Set CreatedObject Script
            if(currentPointingObject == null)
            {
                pointingObjectScript = null;
            }
            else
            {
                pointingObjectScript = currentPointingObject.GetComponent<CreatedObject>();
            }
            
            // Green: Movable / Yellow: Not movable (created by other)
            if(pointingObjectScript != null && pointingObjectScript.Ownership != ownershipName)
            {
                debugMaterial.color = Color.yellow;
            }
            else
            {
                debugMaterial.color = Color.green;
            }
            
            selectedMovableObject = currentPointingObject;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(pv.IsMine == false || moveScript.isMovable == false) return;


        if(movingObject != null && Input.mouseScrollDelta != Vector2.zero)
        {
            objectRotation += Input.mouseScrollDelta.y * Time.deltaTime * rotationAmount;
        }

        isThrow = Input.GetKey(KeyCode.LeftShift);

        CheckObject();

        // Mouse input
        // Press 'X ': Create Object
        if(Input.GetKeyDown(KeyCode.X))
        {
            objectRotation = 0;
            CreateObject();
        }

        // Press 'Z': Delete
        // You can't move object which doesn't have CreatedObject script.
        if(Input.GetKeyDown(KeyCode.Z))
        {
            if(pointingObjectScript == null)
            {
                Debug.LogWarning("Can't destroy: This object was made by default.");
                return;
            }
            if(pointingObjectScript.Ownership != ownershipName)
            {
                Debug.LogWarning("Can't destroy: This object is created by [" + pointingObjectScript.Ownership + "]");
                return;
            }

            //Destroy(selectedMovableObject);
            PhotonNetwork.Destroy(selectedMovableObject.GetComponent<PhotonView>());
        }

        // Right Click: Move movable object
        // You can move object which doesn't have CreatedObject script.
        if(Input.GetMouseButtonDown(1) && selectedMovableObject != null)
        {
            // If the object is created by other player: Can't move
            if(pointingObjectScript != null && pointingObjectScript.Ownership != ownershipName)
            {
                Debug.LogWarning("Can't move: This object is created by [" + pointingObjectScript.Ownership + "]");
                return;
            }

            isMoving = true;
            movingObject = selectedMovableObject;
            movingObjectRigidbody = movingObject.GetComponent<Rigidbody>();

            // Set layer
            movingObject.layer = movingObjectLayer;
            foreach(Transform child in movingObject.transform)
            {
                child.gameObject.layer = movingObjectLayer;
            }

            moveScript.lowPolyAnimationScript.SetCarry(true);
        }

        // Right click release: stop moving object
        if(Input.GetMouseButtonUp(1) && isMoving == true)
        {
            isMoving = false;
            
            if(isThrow == true)
            {
                Rigidbody rigidbody = movingObject.GetComponent<Rigidbody>();
                if(rigidbody == null) return;

                rigidbody.velocity = raycastOrigin.forward * throwForce;
            }
            
            // Set layer
            movingObject.layer = movableObjectLayer;
            foreach(Transform child in movingObject.transform)
            {
                child.gameObject.layer = movableObjectLayer;
            }
            movingObject = null;
            
            moveScript.lowPolyAnimationScript.SetCarry(false);
        }
        if(isMoving == true)
        {
            MoveObject();
        }
    }
}
