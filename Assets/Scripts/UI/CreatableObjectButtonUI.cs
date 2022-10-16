using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CreatableObjectButtonUI : MonoBehaviour
{
    public GameObject buttonPrefab;

    ObjectEditor objectEditorScript;

    void OnClick(int index)
    {
        print(index);
        objectEditorScript.objectIndex = index;
    }

    public void SetButton(ObjectEditor objectEditor, List<GameObject> creatableObjects)
    {
        objectEditorScript = objectEditor;

        for(int i = 0; i < creatableObjects.Count; i++)
        {
            int x = i;
            var button = Instantiate(buttonPrefab);
            button.transform.SetParent(transform);

            button.GetComponent<Button>().onClick.AddListener(delegate{OnClick(x);});
            button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = creatableObjects[x].name;
        }
    }
}
