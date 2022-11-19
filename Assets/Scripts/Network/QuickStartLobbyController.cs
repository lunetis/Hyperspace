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
    

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        quickStartButton.SetActive(true);
        //QuickStart();
        Debug.Log("Connected to master");
    }

    public void QuickStart()
    {
        Debug.Log("Quick start");
        quickStartButton.GetComponent<Button>().interactable = false;
        quickCancelButton.GetComponent<Button>().interactable = true;
        Debug.Log(Text.text);
        Debug.Log("Create or Join room now");
        RoomOptions roomOps = new RoomOptions() {IsVisible = true, IsOpen=true, MaxPlayers=(byte)RoomSize};
        roomOps.CustomRoomProperties = new Hashtable(){{"roomCode",Text.text}};
        PhotonNetwork.JoinOrCreateRoom("Room" +"1" + Text.text, roomOps,null);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to join room... trying again");
        PhotonNetwork.JoinRoom("Room" +"1" + Text.text);
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        RoomOptions roomOps = new RoomOptions() {IsVisible = true, IsOpen=true, MaxPlayers=(byte)RoomSize};
        Debug.Log("Failed to create room... trying again");
        PhotonNetwork.CreateRoom("Room" +"1" + Text.text, roomOps,null);
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
        Debug.Log("Joined <Room" +"1" + Text.text+">");
        if(PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Starting Game");
            PhotonNetwork.LoadLevel(firstSceneIndex);
        }
    }
}