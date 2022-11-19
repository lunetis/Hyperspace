using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class QuickStartLobbyController : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject createRoomButton;    
    [SerializeField]
    private GameObject enterRoomButton;
    [SerializeField]
    private GameObject quickCancelButton;
    [SerializeField]
    private int roomSize;
    private bool enterRoomFlag;
    [SerializeField]
    private int firstSceneIndex;

    public TextMeshProUGUI Text;
    string roomCode;
    

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        enterRoomFlag=false;
        createRoomButton.SetActive(true);
        enterRoomButton.SetActive(true);
        //QuickStart();
        Debug.Log("Connected to master");
    }

    public void CreateRoomButton()
    {
        createRoomButton.GetComponent<Button>().interactable = false;
        enterRoomButton.GetComponent<Button>().interactable = false;
        quickCancelButton.GetComponent<Button>().interactable = true;
        enterRoomFlag=false;

        roomCode = Text.text.Trim();
        
        RoomOptions roomOps = new RoomOptions() {IsVisible = true, IsOpen=true, MaxPlayers=(byte)roomSize};
        roomOps.CustomRoomProperties = new Hashtable(){{"roomCode", roomCode}};

        // 플레이어가 방을 나갈 때 그 플레이어가 생성한 오브젝트 삭제 방지
        roomOps.CleanupCacheOnLeave = false;
        PhotonNetwork.JoinOrCreateRoom(Hyperspace.Utils.GetRoomCode(firstSceneIndex, roomCode), roomOps,null);
    }
    public void EnterRoomButton()
    {
        createRoomButton.GetComponent<Button>().interactable = false;
        enterRoomButton.GetComponent<Button>().interactable = false;
        quickCancelButton.GetComponent<Button>().interactable = true;
        enterRoomFlag=true;

        roomCode = Text.text.Trim();
        
        PhotonNetwork.JoinRoom(Hyperspace.Utils.GetRoomCode(firstSceneIndex, roomCode));
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        if(enterRoomFlag==true){
            Debug.Log("Wrong Room Code(There is no such room");
            enterRoomFlag=false;
            return ;
        }
        PhotonNetwork.JoinRoom(Hyperspace.Utils.GetRoomCode(firstSceneIndex, roomCode));
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        RoomOptions roomOps = new RoomOptions() {IsVisible = true, IsOpen=true, MaxPlayers=(byte)roomSize};
        PhotonNetwork.CreateRoom(Hyperspace.Utils.GetRoomCode(firstSceneIndex, roomCode), roomOps,null);
    }

    public void QuickCancel()
    {
        /*createRoomButton.GetComponent<Button>().interactable = true;
        createRoomButton.GetComponent<Button>().interactable = true;
        quickCancelButton.GetComponent<Button>().interactable = false;
        PhotonNetwork.LeaveRoom();*/
        ///그냥 exit program not leave room
        Application.Quit();
    }

    /*public override void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }*/

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined <" + Hyperspace.Utils.GetRoomCode(firstSceneIndex, roomCode) + ">");
        if(PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(firstSceneIndex);
        }
    }
}