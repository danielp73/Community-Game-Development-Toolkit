using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PreserveOnSceneChange : MonoBehaviour
{
    private GameObject setPlayerLocation;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // called second
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        setPlayerLocation = GameObject.Find("PlayerLocation");

        if (setPlayerLocation != null)
        {
            Debug.LogWarning("setting player location in scene: " + scene.name);

            CharacterController cc = GetComponent<CharacterController>();
            cc.enabled = false;
            transform.position = setPlayerLocation.transform.position;
            cc.enabled = true;

            Debug.LogWarning("setting position to: " + transform.position);
        }
        else
        {
            Debug.LogWarning("didn't find location");
        }

    

        foreach (string tag in GetComponent<PlayerInventoryGrab>().inventoryTags)
        {
            SetUIText(tag, GetComponent<PlayerInventoryGrab>().inventory[tag]);
        }


    }

    // called first
    void OnEnable()
    {
        //Debug.Log("OnEnable called");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }

    private void SetUIText(string inventoryTag, int value)
    {
        Debug.Log("looking for text UI: " + inventoryTag);
        //Text theText = GameObject.Find(inventoryTag + "Count").GetComponent<Text>();
        GameObject inventoryTextBox = GameObject.Find("Canvas/" + inventoryTag + "Count");
        if (inventoryTextBox)
        {
            inventoryTextBox.GetComponent<Text>().text = value.ToString();
            Debug.Log("found text UI");

        }
        //if (theText)
        //{
        //    Debug.Log("found text UI");
        //    theText.text = inventoryTag + ": " + value;
        //}
    }


}
