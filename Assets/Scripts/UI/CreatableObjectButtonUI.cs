using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CreatableObjectButtonUI : MonoBehaviour
{
    ObjectEditor objectEditorScript;

    public GameObject buttonPrefab;
    public TextMeshProUGUI selectedObjectText;
    public Transform objectSelectPopup;
    public List<GameObject> creatableObjects;

    void OnClick(int index, string nameText)
    {
        objectEditorScript.objectIndex = index;
        selectedObjectText.text = nameText;
    }

    public void SetButton(ObjectEditor objectEditor)
    {
        objectEditorScript = objectEditor;
        objectEditor.creatableObjects = creatableObjects;

        for(int i = 0; i < creatableObjects.Count; i++)
        {
            int x = i;
            string nameText = creatableObjects[x].name;
            var button = Instantiate(buttonPrefab);
            button.transform.SetParent(objectSelectPopup);

            button.GetComponent<Button>().onClick.AddListener(delegate{OnClick(x, nameText);});
            button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = nameText;
        }
    }
}
