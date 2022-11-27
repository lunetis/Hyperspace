using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TouchPointer : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public RawImage m_RawImage;
    public Texture m_Texture;

    // Start is called before the first frame update
    void Start()
    {
        m_RawImage = GetComponent<RawImage>();
        m_Texture = GetComponent<Texture>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnPointerDown(PointerEventData data)
    {

        
        m_RawImage.texture = m_Texture;

    }
    public void OnPointerUp(PointerEventData data)
    {
//      img = (RawImage)ImageOnPanel.GetComponent<RawImage>();
//      img.texture = (Texture)NewTexture;
    }
}
