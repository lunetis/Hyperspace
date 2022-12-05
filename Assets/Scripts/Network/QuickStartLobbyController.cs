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
    [SerializeField] private GameObject createRoomButton;    
    [SerializeField] private GameObject enterRoomButton;
    [SerializeField] private GameObject quickCancelButton;
    private int roomSize=20;
    [SerializeField] string username;
    private bool enterRoomFlag;
    [SerializeField]
    private int firstSceneIndex;


    public TextMeshProUGUI Text;
    public TextMeshProUGUI CharacterText;
    string roomCode;
    string playerPrefabName;
    string temp;

    public void UsernameOnValueChange(string valueIn)
    {
        username = valueIn;
        //Debug.Log(username);
    }
    public void RoomSizeOnValueChange(string valueIn)
    {
        if(valueIn=="")
            roomSize=20;
        else
            roomSize = int.Parse(valueIn);
        Debug.Log(roomSize);
    }
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

        SetPlayerCustomProperties();
        
        
        CreateRoom();
    }

    public void EnterRoomButton()
    {
        createRoomButton.GetComponent<Button>().interactable = false;
        enterRoomButton.GetComponent<Button>().interactable = false;
        quickCancelButton.GetComponent<Button>().interactable = true;
        enterRoomFlag=true;

        roomCode = Text.text.Trim();
        
        SetPlayerCustomProperties();
        
        PhotonNetwork.JoinRoom(Hyperspace.Utils.GetRoomCode(firstSceneIndex, roomCode));
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        if(enterRoomFlag==true){
            Debug.Log("Wrong Room Code(There is no such room");
            enterRoomFlag=false;
            createRoomButton.GetComponent<Button>().interactable = true;
            enterRoomButton.GetComponent<Button>().interactable = true;
            quickCancelButton.GetComponent<Button>().interactable = true;
            return ;
        }
        PhotonNetwork.JoinRoom(Hyperspace.Utils.GetRoomCode(firstSceneIndex, roomCode));
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        SetPlayerCustomProperties();
        CreateRoom();
    }


    // 플레이어가 선택한 캐릭터 정보 저장
    void SetPlayerCustomProperties()
    {
        playerPrefabName = CharacterText.text;
        if(username=="")
        {
            username="temp";
        }
        if(playerPrefabName=="Select Character")
            playerPrefabName= string.Format("Player{0}", Random.Range(1, 13));
        //Debug.Log(playerPrefabName);
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable(){{"playerPrefabName", playerPrefabName},{"userName",username}});
        
    }

    void CreateRoom()
    {
        RoomOptions roomOps = new RoomOptions() {IsVisible = true, IsOpen=true, MaxPlayers=(byte)roomSize};
        roomOps.CustomRoomProperties = new Hashtable(){{"roomCode", roomCode},{"roomChat",""},{"roomSize",roomSize}};

        // 플레이어가 방을 나갈 때 그 플레이어가 생성한 오브젝트 삭제 방지
        roomOps.CleanupCacheOnLeave = false;
        PhotonNetwork.JoinOrCreateRoom(Hyperspace.Utils.GetRoomCode(firstSceneIndex, roomCode), roomOps,null);
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
        Debug.Log(PhotonNetwork.LocalPlayer.CustomProperties["userName"].ToString());
        if(PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(firstSceneIndex);
        }
    }

    public static bool IsHost()
    {
        return (PhotonNetwork.MasterClient == PhotonNetwork.LocalPlayer);
    }
}