using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
               

public class CharacterSelectPanel : MonoBehaviour
{
    public List<GameObject> characterList;
    public GameObject buttonPrefab;
    public Transform contentUI;

    public TextMeshProUGUI buttonText;
    public Transform playerSpawnPosition;

    // Start is called before the first frame update
    void Start()
    {
        foreach(var character in characterList)
        {
            GameObject button = Instantiate(buttonPrefab, Vector3.zero, Quaternion.identity);
            button.transform.parent = contentUI;
            button.transform.GetComponentInChildren<TextMeshProUGUI>().text = character.name;

            button.GetComponent<Button>().onClick.AddListener(delegate { OnClick(character.name); });
        }

        gameObject.SetActive(false);
    }


    void OnClick(string characterName)
    {
        buttonText.text = characterName;
        gameObject.SetActive(false);

        // 이전 오브젝트 삭제
        if(playerSpawnPosition.childCount > 0)
        {
            Destroy(playerSpawnPosition.GetChild(0).gameObject);
        }


        // null에 플레이어 프리팹 등록
        GameObject playerObject = null;
        foreach (var character in characterList)
        {
            if(characterName == character.name)
            {
                playerObject = character;
            }
            character.GetComponent<MoveScript>().enabled = false;
        }
        //GameObject playerObject = null;

        GameObject playerExample = Instantiate(playerObject, playerSpawnPosition.transform.position, Quaternion.identity);
        playerExample.transform.parent = playerSpawnPosition;

    }

    // Update is called once per frame
    void Update()
    {
    }
}
