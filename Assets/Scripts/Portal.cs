using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    public int levelIndex;
    
    [SerializeField]
    private int RoomSize = 10;

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if(other.GetComponent<PhotonView>()?.IsMine == true)
            {
                Debug.Log("Portal Entered + " + levelIndex);
                //PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.LocalPlayer);
                // PhotonNetwork.LeaveRoom();
                PhotonNetwork.Destroy(other.gameObject);
                
                Debug.Log("Creating room now");
                PhotonNetwork.AutomaticallySyncScene = false;
                PhotonNetwork.LoadLevel(levelIndex);

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
}
