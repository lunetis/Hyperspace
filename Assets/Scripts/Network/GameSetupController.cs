using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameSetupController : MonoBehaviour
{
    public Transform spawnPosition;

    void Start()
    {
        CreatePlayer();
    }

    private void CreatePlayer()
    {
        string playerPrefabName = PhotonNetwork.LocalPlayer.CustomProperties["playerPrefabName"].ToString();
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", playerPrefabName), spawnPosition.position, Quaternion.identity);
    }


    public void ExitButton()
    {
        PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.LocalPlayer);
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel(0);
    }
}
