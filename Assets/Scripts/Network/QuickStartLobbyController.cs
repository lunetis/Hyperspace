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
    private GameObject quickStartButton;
    [SerializeField]
    private GameObject quickCancelButton;
    [SerializeField]
    private int RoomSize;
    
    [SerializeField]
    private int firstSceneIndex;

    public TextMeshProUGUI Text;
    string roomCode;
    

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        
        quickStartButton.SetActive(true);
        //QuickStart();
        Debug.Log("Connected to master");
    }

    public void QuickStart()
    {
        quickStartButton.GetComponent<Button>().interactable = false;
        quickCancelButton.GetComponent<Button>().interactable = true;

        roomCode = Text.text.Trim();
        
        RoomOptions roomOps = new RoomOptions() {IsVisible = true, IsOpen=true, MaxPlayers=(byte)RoomSize};
        roomOps.CustomRoomProperties = new Hashtable(){{"roomCode", roomCode}};

        // 플레이어가 방을 나갈 때 그 플레이어가 생성한 오브젝트 삭제 방지
        roomOps.CleanupCacheOnLeave = false;
        PhotonNetwork.JoinOrCreateRoom(Hyperspace.Utils.GetRoomCode(firstSceneIndex, roomCode), roomOps,null);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        PhotonNetwork.JoinRoom(Hyperspace.Utils.GetRoomCode(firstSceneIndex, roomCode));
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        RoomOptions roomOps = new RoomOptions() {IsVisible = true, IsOpen=true, MaxPlayers=(byte)RoomSize};
        PhotonNetwork.CreateRoom(Hyperspace.Utils.GetRoomCode(firstSceneIndex, roomCode), roomOps,null);
    }

    public void QuickCancel()
    {
        quickStartButton.GetComponent<Button>().interactable = true;
        quickCancelButton.GetComponent<Button>().interactable = false;
        PhotonNetwork.LeaveRoom();
        ///그냥 exit program not leave room
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