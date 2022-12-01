using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

[RequireComponent(typeof(TMP_InputField))]
public class ChatInputField : MonoBehaviour
{
    TMP_InputField inputField;

    [HideInInspector]
    public MoveScript localMoveScript;

    // Start is called before the first frame update
    void Start()
    {
        inputField = GetComponent<TMP_InputField>();

        inputField.onSelect.AddListener(DisablePlayerMove);
        inputField.onDeselect.AddListener(EnablePlayerMove);


        var playerPVs = FindObjectsOfType<PhotonView>();
        foreach(var pv in playerPVs)
        {
            if(pv.IsMine == true)
            {
                Debug.Log("Find!");
                localMoveScript = pv.gameObject.GetComponent<MoveScript>();
                break;
            }
        }
    }

    void DisablePlayerMove(string str)
    {
        localMoveScript.isMovable = false;
    }

    void EnablePlayerMove(string str)
    {
        localMoveScript.isMovable = true;
    }
}
