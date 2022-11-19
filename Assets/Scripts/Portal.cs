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
        if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if(other.GetComponent<PhotonView>()?.IsMine == true)
            {
                hashTable = PhotonNetwork.CurrentRoom.CustomProperties;
                //PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.LocalPlayer);
                PhotonNetwork.Destroy(other.gameObject);

                roomCode = (string)hashTable["roomCode"];
                isActivePortal = true;
                PhotonNetwork.LeaveRoom();
            }
        }
    }

    private void OnTriggerExit(Collider other) {
    }

    public override void OnLeftRoom(){
        if(isActivePortal == false) return;
        // Wait for OnConnectedToMaster
    }

    public override void OnConnectedToMaster()
    {
        if(isActivePortal == false) return;

        RoomOptions roomOps = new RoomOptions() {IsVisible = true, IsOpen=true, MaxPlayers=(byte)RoomSize};
        roomOps.CustomRoomProperties = new Hashtable(){{"roomCode", roomCode}};
        
        // 플레이어가 방을 나갈 때 그 플레이어가 생성한 오브젝트 삭제 방지
        roomOps.CleanupCacheOnLeave = false;
        PhotonNetwork.JoinOrCreateRoom(Hyperspace.Utils.GetRoomCode(levelIndex, roomCode), roomOps,null);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        if(isActivePortal == false) return;
        PhotonNetwork.JoinRoom(Hyperspace.Utils.GetRoomCode(levelIndex, roomCode));
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        if(isActivePortal == false) return;
        RoomOptions roomOps = new RoomOptions() {IsVisible = true, IsOpen=true, MaxPlayers=(byte)RoomSize};
        PhotonNetwork.CreateRoom(Hyperspace.Utils.GetRoomCode(levelIndex, roomCode), null);
    }
    public override void OnJoinedRoom()
    {
        if(isActivePortal == false) return;
        Debug.Log("Joined <" + Hyperspace.Utils.GetRoomCode(levelIndex, roomCode) +">");
        
        if(PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(levelIndex);
        }
    }
}
