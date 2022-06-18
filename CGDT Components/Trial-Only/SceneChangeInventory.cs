using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeInventory : MonoBehaviour
{
    [System.Serializable]
    public struct inventoryChecks
    {
        public string inventoryTag;
        public int amountOfInventory;
    }
    public inventoryChecks[] inventoryChecksArray;

    public GameObject thePlayer;
    public GameObject gameTransitionImage;
    public string sceneToLoad;
    private bool gameTransition = false;
    public string playerTag = "Player";

    // Start is called before the first frame update
    void Start()
    {
        gameTransitionImage.SetActive(false);
        if (sceneToLoad == "")
        {
            sceneToLoad = SceneManager.GetActiveScene().path;
        }


    }

    // Update is called once per frame
    void Update()
    {
 
    }


    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("scene change inv: trigger enter");
        if (other.gameObject.tag == playerTag)
        {

            Dictionary<string, int> inventory;

            inventory = thePlayer.GetComponent<PlayerInventoryGrab>().inventory;

            //inventory = thePlayer.inventory();
            //inventory = GetComponent<PlayerInventoryGrab>().inventory;


            bool inventoryCheck = true;

            for (int i = 0; i < inventoryChecksArray.Length; i++)
            {
                //see if player has this inventory tag in first place
                if (inventory.ContainsKey(inventoryChecksArray[i].inventoryTag))
                {
                    //Debug.Log("player has inventory tag: " + inventoryChecksArray[i].inventoryTag);

                    //has that inventory tag

                    //check  inventory amount
                    int getInventory = inventory[inventoryChecksArray[i].inventoryTag];

                    //compare it to required amount
                    if (getInventory < inventoryChecksArray[i].amountOfInventory)
                    {
                        //Debug.Log("player doesn't have enough: " + inventoryChecksArray[i].inventoryTag);
                        //had that inventory key, but not enough of it
                        inventoryCheck = false;
                    }
                    else
                    {
                        Debug.Log("player has enough: " + inventoryChecksArray[i].inventoryTag);
                    }
                }
                else
                {
                    //Debug.Log("player doesn't have tag: " + inventoryChecksArray[i].inventoryTag);
                    //didn't have that inventory item at all
                    inventoryCheck = false;
                }
            }

            if (inventoryCheck)
            {
                Debug.Log("game transition: inventory check");

                if (gameTransitionImage)
                {
                    gameTransition = true;
                    gameTransitionImage.SetActive(true);
                    Time.timeScale = 0;
                }
                else
                {
                    SceneManager.LoadScene(sceneToLoad);
                }

            }

            //Debug.Log("game end: tag match");
            //if (gameTransitionImage)
            //{
            //    gameTransition = true;
            //    gameTransitionImage.SetActive(true);
            //    Time.timeScale = 0;
            //}
            //else
            //{
            //    SceneManager.LoadScene(sceneToLoad);
            //}
        }
    }



    private void OnGUI()
    {
        if (gameTransition)
        {
            Event e = Event.current;
            if (e.keyCode == KeyCode.Return)
            {
                gameTransition = false;
                SceneManager.LoadScene(sceneToLoad);
                Time.timeScale = 1;
            }
        }

    }

}
