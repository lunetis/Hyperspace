using UnityEngine;

using Photon.Pun;

public class MoveScript : MonoBehaviourPunCallbacks
{
    public CharacterController controller;
    public CameraController cameraController;
    
    public float Speed;
    public float JumpPow;

    private float Gravity;
    private Vector3 direction;

    public float mouseSensitivity;

    private Vector3 rotation;
    public float clampAngle;

    public Transform cameraPivot;

    private PhotonView pv;

    public Vector3 LookRotation
    {
        get { return rotation; }
    }

 
    // Start is called before the first frame update
    void Start()
    {
        if(controller == null)
        {
            controller = GetComponent<CharacterController>();
        }

        if(cameraController == null)
        {
            cameraController = GetComponent<CameraController>();
        }

        Speed = 5.0f;
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

    // Update is called once per frame
    void Update()
    {
        if(pv.IsMine == true)
        {
            // Look
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = -Input.GetAxis("Mouse Y");

            rotation.y += mouseX * mouseSensitivity * Time.deltaTime;
            rotation.x += mouseY * mouseSensitivity * Time.deltaTime;

            rotation.x = Mathf.Clamp(rotation.x, -clampAngle, clampAngle);

            Quaternion localRotation = Quaternion.Euler(0, rotation.y, 0);

            // Camera rotation
            cameraPivot.localRotation = Quaternion.Euler(rotation.x, 0, 0);
            transform.rotation = localRotation;

            // Move
            direction.x = Input.GetAxis("Horizontal") * Speed;
            direction.z = Input.GetAxis("Vertical") * Speed;
            direction = controller.transform.TransformDirection(direction);
            if (controller.isGrounded)
            {
                if (Input.GetButtonDown("Jump"))
                {
                    direction.y = JumpPow;
                }
            }
            else
            {         
                direction.y -= Gravity * Time.deltaTime;
            }
        }
        
        controller.Move(direction * Time.deltaTime);
    }
}
