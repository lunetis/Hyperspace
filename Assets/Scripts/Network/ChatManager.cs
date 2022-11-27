using ExitGames.Client.Photon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Chat;
using Photon.Pun;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour, IChatClientListener
{
    #region Setup

    [SerializeField] GameObject joinChatButton;
    ChatClient chatClient;
    bool isConnected;
    [SerializeField] string username;

    public void UsernameOnValueChange(string valueIn)
    {
        username = valueIn;
        //Debug.Log(username);
    }
    
    public void ChatConnectOnClick()
    {
        Debug.Log("Connecting");
        chatClient.ChatRegion = "asia";
        chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat,PhotonNetwork.AppVersion,
         new Photon.Chat.AuthenticationValues(username));
    }

    #endregion Setup

    #region General

    [SerializeField] GameObject chatPanel;
    string privateReceiver= "";
    string currentChat;
    //[SerializeField] InputField chatField;
    //[SerializeField] Text chatDisplay;
    // Start is called before the first frame update
    void Start()
    {
        chatClient = new ChatClient(this);
        if (string.IsNullOrEmpty(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat))
            Debug.Log("no CHat app ID provided");
    }
    // Update is called once per frame
    void Update()
    {
       chatClient.Service();

       /*if(chatField.text!="" && Input.GetKey(KeyCode.Return))
       {
            //SubmitPublicChatOnClick();
            //SubmitPrivateChatOnClick();
            ;
       }*/
    }

    #endregion General

    public void DebugReturn(DebugLevel level, string message)
    {
        // throw new System.NotImplementedException();
    }
    public void OnChatStateChange(ChatState state)
    {
        // throw new System.NotImplementedException();
    }
    public void OnConnected()
    {
        Debug.Log("Connected");
        isConnected=true;
        joinChatButton.SetActive(false);
        chatClient.Subscribe(new string[] {"RegionChannel"});
        //SubToChatOnClick();
    }
    public void OnDisconnected()
    {
        //Debug.Log("Connect failed");
        throw new System.NotImplementedException();
    }
    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        throw new System.NotImplementedException();
    }
    public void OnPrivateMessage(string sender, object messages,string channelName)
    {
        throw new System.NotImplementedException();
    }
    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        throw new System.NotImplementedException();
    }
    public void OnSubscribed(string[] channels, bool[] results)
    {
        chatPanel.SetActive(true);
    }
     public void OnUnsubscribed(string[] channels)
    {
        throw new System.NotImplementedException();
    }
    public void OnUserSubscribed(string channels, string user)
    {
        throw new System.NotImplementedException();
    }
    public void OnUserUnsubscribed(string channels, string user)
    {
        throw new System.NotImplementedException();
    }
}
