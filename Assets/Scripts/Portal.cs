using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Portal : MonoBehaviour
{
    public int levelIndex;

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if(other.GetComponent<PhotonView>()?.IsMine == true)
            {
                Debug.Log("Portal Entered + " + levelIndex);
                PhotonNetwork.Destroy(other.gameObject);
                PhotonNetwork.LoadLevel(levelIndex);
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        Debug.Log("Portal Leaved");
    }
}
