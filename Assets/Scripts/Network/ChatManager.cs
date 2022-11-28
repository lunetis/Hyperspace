using ExitGames.Client.Photon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Chat;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;

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
    [SerializeField] TMP_InputField chatField;
    [SerializeField] TMP_Text chatDisplay;
    string privateReceiver= "";
    string currentChat;

    void Start()
    {
        if (string.IsNullOrEmpty(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat))
            Debug.Log("no Chat app ID provided");
        chatClient = new ChatClient(this);
    }

    void Update()
    {
       chatClient.Service();

       if(chatField.text!="" && Input.GetKey(KeyCode.Return))
       {
            SendChatOnClick();
       }
    }

    public void TypeChatOnValueChange(string valueIn)
    {
        currentChat=valueIn;
    }

    public void ReceiverOnValueChange(string valueIn)
    {
        privateReceiver=valueIn;
    }
    public void SendChatOnClick()
    {
        if(privateReceiver=="")
            chatClient.PublishMessage("RegionChannel",currentChat);
        
        if(privateReceiver!="")
            chatClient.SendPrivateMessage(privateReceiver,currentChat);
        chatField.text="";
        currentChat="";
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
        string msgs="";
        for(int i=0;i<senders.Length;i++)
        {
            msgs=string.Format("{0}: {1}",senders[i], messages[i]);
            chatDisplay.text+="\n" + msgs;
            //Debug.Log(msgs);
        }
    }
    public void OnPrivateMessage(string sender, object messages,string channelName)
    {
        string msgs="";
        msgs=string.Format("(Private) {0}: {1}",sender, messages);
        chatDisplay.text+="\n" + msgs;
        //Debug.Log(msgs);
    }
    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        throw new System.NotImplementedException();
    }
    public void OnSubscribed(string[] channels, bool[] results)
    {
        Debug.Log("Subscribed");
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