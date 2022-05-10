
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryAnimationTrigger : MonoBehaviour
{

    Animator animator;
    bool doorOpen;

    [System.Serializable]
    public struct inventoryChecks
    {
        public string inventoryTag;
        public int amountOfInventory;
    }
    public inventoryChecks[] inventoryChecksArray;
    private Dictionary<string, int> inventoryChecksDict;

    private void Start()
    {
        doorOpen = false;
        animator = GetComponent<Animator>();

        for(int i=0; i< inventoryChecksArray.Length; i++)
        {
            inventoryChecksDict.Add(inventoryChecksArray[i].inventoryTag, inventoryChecksArray[i].amountOfInventory);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("trigger enter tag: " + openingTag);
        Debug.Log("trigger enter: " + other.gameObject.tag);


        if (other.gameObject.tag == "Player")
        {
            PlayerInventoryGrab theInventory = other.gameObject.GetComponent<PlayerInventoryGrab>();

            bool inventoryCheck = true;

            for (int i = 0; i < inventoryChecksArray.Length; i++)
            {
                //see if player has this inventory tag in first place
                if (theInventory.inventory.ContainsKey(inventoryChecksArray[i].inventoryTag)) {
                    Debug.Log("player has inventory tag: " + inventoryChecksArray[i].inventoryTag);

                    //has that inventory tag

                    //check  inventory amount
                    int getInventory = theInventory.inventory[inventoryChecksArray[i].inventoryTag];

                    //compare it to required amount
                    if (getInventory < inventoryChecksArray[i].amountOfInventory)
                    {
                        Debug.Log("player doesn't have enough: " + inventoryChecksArray[i].inventoryTag);
                        //had that inventory key, but not enough of it
                        inventoryCheck = false;
                    } else
                    {
                        Debug.Log("player has enough: " + inventoryChecksArray[i].inventoryTag);
                    }
                } else
                {
                    Debug.Log("player doesn't have tag: " + inventoryChecksArray[i].inventoryTag);
                    //didn't have that inventory item at all
                    inventoryCheck = false;
                }
            }

            if(inventoryCheck)
            {
                doorOpen = true;
                SetTrigger("Open");
                Debug.Log("door opening");
            }


        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (doorOpen)
        {
            doorOpen = false;
            SetTrigger("Close");
            Debug.Log("door closing");
        }
    }

    void SetTrigger(string direction)
    {
        Debug.Log("setting trigger: " + direction);
        animator.SetTrigger(direction);
    }
}
