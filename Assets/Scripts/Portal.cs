using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;
public class Portal : MonoBehaviourPunCallbacks
{
    public int levelIndex;
    
    [SerializeField]
    private int RoomSize = 10;
    private string roomCode;
    private Hashtable hashTable;

    private bool isActivePortal;

    private void OnTriggerEnter(Collider other) {
        hashTable = PhotonNetwork.CurrentRoom.CustomProperties;
        if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if(other.GetComponent<PhotonView>()?.IsMine == true)
            {
                Debug.Log("Portal Entered + " + levelIndex);
                //PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.LocalPlayer);
                PhotonNetwork.Destroy(other.gameObject);
                roomCode=(string)hashTable["roomCode"];
                Debug.Log("Leave room now");
                isActivePortal = true;
                PhotonNetwork.LeaveRoom();
                //PhotonNetwork.AutomaticallySyncScene = false;
                // SceneManager.LoadScene(levelIndex);
                // RoomOptions roomOps = new RoomOptions() {IsVisible = true, IsOpen=true, MaxPlayers=(byte)RoomSize};

                // TypedLobby typedLobby = new TypedLobby("Lobby", LobbyType.Default);
                // PhotonNetwork.JoinOrCreateRoom("Room" + levelIndex, roomOps, typedLobby);
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        Debug.Log("Portal Leaved");
    }

    public override void OnLeftRoom(){
        if(isActivePortal == false) return;
        Debug.Log("OnLeftRoom");

        // Wait for OnConnectedToMaster
    }

    public override void OnConnectedToMaster()
    {
        //QuickStart();
        Debug.Log("Connected to master");

        RoomOptions roomOps = new RoomOptions() {IsVisible = true, IsOpen=true, MaxPlayers=(byte)RoomSize};
        roomOps.CustomRoomProperties = new Hashtable(){{"roomCode",roomCode}};

        PhotonNetwork.JoinOrCreateRoom("Room" +levelIndex + roomCode, roomOps,null);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        if(isActivePortal == false) return;
        Debug.Log("Failed to join room... trying again");
        PhotonNetwork.JoinRoom("Room" +"1" + roomCode);
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        if(isActivePortal == false) return;
        RoomOptions roomOps = new RoomOptions() {IsVisible = true, IsOpen=true, MaxPlayers=(byte)RoomSize};
        Debug.Log("Failed to create room... trying again");
        PhotonNetwork.CreateRoom("Room" +"1" + roomCode, roomOps,null);
    }
    public override void OnJoinedRoom()
    {
        if(isActivePortal == false) return;
        Debug.Log("Joined <Room" +levelIndex + roomCode+">");
        
        if(PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(levelIndex);
        }
    }
}
