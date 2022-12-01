using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class MoveScript : MonoBehaviourPunCallbacks
{
    public CharacterController controller;
    public CameraController cameraController;
    
    public float Speed;
    public float RunSpeed;
    public float JumpPow;

    private float Gravity;
    private Vector3 direction;

    public float mouseSensitivity;

    private Vector3 rotation;
    float clampAngle = 30;

    public Transform cameraPivot;

    private PhotonView pv;

    public float pushForce = 3;

    public LowPolyAnimationScript lowPolyAnimationScript;

    bool isRunning;

    [HideInInspector]
    public bool isMovable;


    public Vector3 LookRotation
    {
        get { return rotation; }
    }

    void AttachCameraPivot()
    {
        Transform mapCameraPivot = FindObjectOfType<MapController>().cameraPivot;
        mapCameraPivot.parent = cameraPivot;
        mapCameraPivot.transform.localPosition = Vector3.zero;

        foreach(Transform child in mapCameraPivot)
        {
            Camera cam = child.GetComponent<Camera>();
            if(cam != null)
            {
                cameraController.cameras.Add(cam);
            }
        }
    }


    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody rb = hit.collider.attachedRigidbody;
        if(rb != null && !rb.isKinematic)
        {
            rb.velocity = hit.moveDirection * pushForce;
        }
    }


    void Animate(float moveSpeed)
    {
        if(lowPolyAnimationScript == null)
            return;

        lowPolyAnimationScript.Animate(moveSpeed, isRunning);
    }

 
    // Start is called before the first frame update
    void Start()
    {
        isMovable = true;

        if(controller == null)
        {
            controller = GetComponent<CharacterController>();
        }

        if(cameraController == null)
        {
            cameraController = GetComponent<CameraController>();
        }

        Gravity = 10.0f;
        direction = Vector3.zero;
        JumpPow = 5.0f;

        rotation = transform.localRotation.eulerAngles;
        rotation.x = 0;

        pv = GetComponent<PhotonView>();
        if(pv.IsMine == true)
        {
            AttachCameraPivot();
        }
        else
        {
            controller.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(pv.IsMine == false) return;

        // Look
        float mouseX = (isMovable == true) ? Input.GetAxis("Mouse X") : 0;
        float mouseY = (isMovable == true) ? -Input.GetAxis("Mouse Y") : 0;

        rotation.y += mouseX * mouseSensitivity * Time.deltaTime;
        rotation.x += mouseY * mouseSensitivity * Time.deltaTime;

        if(cameraController.index == 0)
        {
            rotation.x = Mathf.Clamp(rotation.x, -60, 80);
        }
        else
        {
            rotation.x = Mathf.Clamp(rotation.x, -30, 80);
        }
        

        Quaternion localRotation = Quaternion.Euler(0, rotation.y, 0);

        // Camera rotation
        cameraPivot.localRotation = Quaternion.Euler(rotation.x, 0, 0);
        transform.rotation = localRotation;

        // Move
        // Run
        isRunning = Input.GetKey(KeyCode.LeftShift);
        float moveSpeed = (isRunning == true) ? RunSpeed : Speed;

        direction.x = (isMovable == true) ? Input.GetAxis("Horizontal") * Speed : 0;
        direction.z = (isMovable == true) ? Input.GetAxis("Vertical") * moveSpeed : 0;

        // This movespeed will be passed to Animate()
        moveSpeed = Mathf.Max(Mathf.Abs(direction.x), Mathf.Abs(direction.z));
        direction = controller.transform.TransformDirection(direction);
        if (controller.isGrounded)
        {
            if (Input.GetButtonDown("Jump") && isMovable == true)
            {
                direction.y = JumpPow;
            }
        }
        else
        {         
            direction.y -= Gravity * Time.deltaTime;
        }
        controller.Move(direction * Time.deltaTime);
        
        // Animation
        Animate(moveSpeed);

        // Key Input
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            lowPolyAnimationScript.SetMotion(1);
        }
        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            lowPolyAnimationScript.SetMotion(2);
        }
        if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            lowPolyAnimationScript.SetMotion(3);
        }

        // Camera
        if(Input.GetButtonDown("Camera"))
        {
            cameraController.SetCamera();
        }
    }

    private void OnApplicationQuit() {
        PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.LocalPlayer);
        PhotonNetwork.Destroy(pv);
        PhotonNetwork.LeaveRoom();
    }
}
