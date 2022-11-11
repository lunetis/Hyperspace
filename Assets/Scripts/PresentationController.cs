using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;
using System.Linq;
using SFB;

// using Photon.Pun;
// using Photon.Realtime;

public class PresentationData
{    
    public Texture2D slideTexture;
    public int slideEmoteIndex = 0;
}

public class PresentationController : MonoBehaviour // : MonoBehaviourPunCallbacks
{
    [SerializeField]
    MeshRenderer screenRenderer;
    [SerializeField]
    TextMeshProUGUI slideText;

    [SerializeField]
    SlideSettingsUIController slideSettingsUI;
    public Button nextSlideButton;
    public Button prevSlideButton;


    [Header("Presentation Only Mode")]
    public RawImage keynoteRenderImage;
    
    // [HideInInspector]
    // public FaceController faceController;

    public GameObject presenterUI;

    public bool isReady = false;

    // public PhotonView pv;



    // Internal data
    string[] slidePaths;

    // Use preload textures: Load when import folder
    public List<PresentationData> originalDataList;
    public List<PresentationData> presentationDataList;
    

    int index = 0;
    int maxIndex = -1;
    PresentationData currentData;

    public static IEnumerable<string> GetSlidesFromFolder(string folderPath)
    {
        // Filter: png, jpg
        var paths = Directory.EnumerateFiles(folderPath, "*.*", SearchOption.TopDirectoryOnly);
        return paths.Where(s => s.EndsWith(".png") || s.EndsWith(".PNG") || s.EndsWith(".jpg"));
    }


    // Start is called before the first frame update
    void Start()
    {
        // if(PresentationController.IsHost() == false)
        // {
        //     presenterUI.SetActive(false);
        //     // Request current slide if guest
        //     StartCoroutine(RequestSlideData());
        //     return;
        // }

        slideText.text = "Press \"Open Folder\" to import";

        // Is there any data in PresentationDataObject?
        if(PresentationDataObject.data != null)
        {
            SetDataFromSelectScene();
        }
        
        // if(pv == null)
        // {
        //     pv = GetComponent<PhotonView>();
        // }
    }


    void Update()
    {
        if(slideSettingsUI != null && slideSettingsUI.gameObject.activeSelf == true)
        {
            return;
        }

        if(Input.GetKeyDown(KeyCode.Return))
        {
            if(Input.GetKey(KeyCode.LeftControl))
            {
                ShowPrevSlide();
            }
            else
            {
                ShowNextSlide();
            }
        }
        
        if(Input.GetKeyDown(KeyCode.Space))
        {
            ShowNextSlide();
        }
    }

    public void ImportFolder()
    {
        string[] paths = StandaloneFileBrowser.OpenFolderPanel("Select Folder", "", true);
        string path = "";
        foreach(string str in paths)
        {
            path += str;
        }

        // Not selected (ex. cancel)
        if(path == "")
        {
            return;
        }

        slidePaths = GetSlidesFromFolder(path).ToArray<string>();
        Debug.Log(slidePaths.Length + " png/jpg files were found.");

        // No images found
        if(slidePaths.Length == 0)
        {
            maxIndex = -1;
            return;
        }

        maxIndex = slidePaths.Length - 1;
        index = 0;

        // Load and store slides
        originalDataList?.Clear();
        originalDataList = new List<PresentationData>();

        foreach(var slidePath in slidePaths)
        {
            var data = new PresentationData();
            data.slideTexture = GetTextureFromImage(slidePath);
            originalDataList.Add(data);
        }
        // Go to slide settings
        slideSettingsUI.Init(originalDataList);
        slideText.text = string.Format("Press 'Apply' to see presentations");
    }


    public void SetDataFromSelectScene()
    {
        originalDataList = PresentationDataObject.data;
        slideSettingsUI.Init(originalDataList);

        PresentationDataObject.data = null;
        
        // Close settings UI
        slideSettingsUI.gameObject.SetActive(false);

        // Apply automatically
        List<int> indices = Enumerable.Range(1, originalDataList.Count).ToList();
        ApplyNewSlideList(indices);
    }

    
    public void ShowNextSlide()
    {
        index = Mathf.Clamp(index + 1, 0, maxIndex);
        ShowSlide();
    }

    public void ShowPrevSlide()
    {
        index = Mathf.Clamp(index - 1, 0, maxIndex);
        ShowSlide();
    }

    // This indices are 1-base (starts with 1, ends with indices.Count)
    public void ApplyNewSlideList(List<int> indices)
    {
        presentationDataList = new List<PresentationData>(indices.Count);
        foreach(int index in indices)
        {
            presentationDataList.Add(originalDataList[index - 1]);
        }
        maxIndex = indices.Count - 1;

        // Show First Slide
        index = 0;
        ShowSlide();
    }

    void ShowSlide()
    {
        // Set Buttons
        prevSlideButton.interactable = (index != 0);
        nextSlideButton.interactable = (index != maxIndex);

        var data = presentationDataList[index];
        currentData = data;

        keynoteRenderImage.texture = data.slideTexture;
        screenRenderer.material.mainTexture = data.slideTexture;
        // Emote play
        // if(faceController != null)
        // {
        //     faceController.PlayFaceAnim(data.slideEmoteIndex);
        // }

        slideText.text = string.Format("Slide {0} / {1}", index + 1, maxIndex + 1);
        isReady = true;


        // Send Texture when changing slides
        // SendTextureToClient();
    }

    Texture2D GetTextureFromImage(string imagePath)
    {
        Texture2D texture;
        if(File.Exists(imagePath))
        {
            texture = new Texture2D(2, 2);
            texture.LoadImage(File.ReadAllBytes(imagePath));
            return texture;
        }
        else
        {
            return null;
        }
    }

    public static bool IsHost()
    {
        return true; // QuickStartLobbyController.host == 1;
    }
    

    // Photon functions
    // IEnumerator RequestSlideData()
    // {
    //     while(PhotonNetwork.NetworkClientState != ClientState.Joined)
    //     {
    //         yield return null;
    //     }
    //     pv.RPC("SendTextureToClient", RpcTarget.MasterClient);
    // }

    // [PunRPC]
    // void SendTextureToClient()
    // {
    //     if(IsHost() == true)
    //     {
    //         pv.RPC("ReceiveTexture", RpcTarget.Others, presentationDataList[index].slideTexture.EncodeToPNG());
    //     }
    // }

    // [PunRPC]
    // void ReceiveTexture(byte[] receivedByte)
    // {
    //     var receivedTexture = new Texture2D(1, 1);
    //     receivedTexture.LoadImage(receivedByte);
    //     ShowSlideWithTexture(receivedTexture);
    // }

    // Warning: Video is not supported
    void ShowSlideWithTexture(Texture2D texture)
    {
        keynoteRenderImage.texture = texture;
        screenRenderer.material.mainTexture = texture;
    }
}
