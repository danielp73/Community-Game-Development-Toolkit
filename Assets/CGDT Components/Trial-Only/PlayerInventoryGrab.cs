using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventoryGrab : MonoBehaviour
{

    // Reference to the character camera.
    [SerializeField]
    private Camera characterCamera;

    // Reference to the slot for holding picked item.
    [SerializeField]
    private Transform slot;

    // Reference to the currently held item.
    private GameObject pickedItem;

    //object distance
    public float maxObjectDistance = 5f;

    public string[] inventoryTags;
	public Dictionary<string, int> inventory;

	// Start is called before the first frame update
	void Start()
	{
		inventory = new Dictionary<string, int>();



		for (int i = 0; i < inventoryTags.Length; i++)
		{
			Debug.Log("adding: " + inventoryTags[i]);
			inventory.Add(inventoryTags[i], 0);
		}
	}

    private void Update()
    {
        // Execute logic only on button pressed
        if (Input.GetButtonDown("Fire1"))
        {
            Debug.Log("pickable: clicked");
            // see if we're holdign something. if so drop i
            if (pickedItem)
            {
                DropItem(pickedItem);
            }
            else
            {
                Debug.Log("pickable: trying raycast");

                // If no, try to pick item in front of the player
                // Create ray from center of the screen
                var ray = characterCamera.ViewportPointToRay(Vector3.one * 0.5f);
                RaycastHit hit;



                if (Physics.Raycast(ray, out hit, maxObjectDistance))
                {
                    Debug.Log("pickable: raycast hit");

                    // Check if object is pickable
                    var pickable = hit.transform.GetComponent<PickableItem>();

                    // If object has PickableItem class
                    if (pickable)
                    {
                        Debug.Log("pickable: it's pickable");

                        // Pick it
                        PickItem(hit.collider.gameObject);
                    }

                    // see if it's a pickup / movable itm.
                    //if (hit.collider.CompareTag(pickableTag))
                    //{
                    //    Debug.Log("clicked pickable item");
                    //    PickItem(hit.collider.gameObject);
                    //}


                    //check item tag
                    for (int i = 0; i < inventoryTags.Length; i++)
                    {
                        Debug.Log("pickable: checking tag: " + inventoryTags[i]);

                        // see if it's an inventory tag
                        if (hit.collider.CompareTag(inventoryTags[i]))
                        {
                            Debug.Log("pickable: tag matched");

                            // Make the other game object (the pick up) inactive, to make it disappear
                            hit.collider.gameObject.SetActive(false);

                            //increase count
                            inventory[inventoryTags[i]] = inventory[inventoryTags[i]] + 1;

                            Debug.Log("picked up: " + inventoryTags[i] + " new amount: :" + inventory[inventoryTags[i]]);

                            SetUIText(inventoryTags[i], inventory[inventoryTags[i]]);
                        }

                    }
                }
            }

        }
		}

    private void SetUIText(string inventoryTag, int value)
    {
        Debug.Log("looking for text UI: " + inventoryTag);
        //Text theText = GameObject.Find(inventoryTag + "Count").GetComponent<Text>();
        GameObject inventoryTextBox = GameObject.Find("Canvas/" + inventoryTag + "Count");
        if(inventoryTextBox)
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

    private void PickItem(GameObject item)
    {
        // Assign reference
        pickedItem = item;

        // Disable rigidbody and reset velocities
        item.GetComponent<Rigidbody>().isKinematic = true;
        item.GetComponent<Rigidbody>().velocity = Vector3.zero;
        item.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

        //item.Rb.isKinematic = true;
        //item.Rb.velocity = Vector3.zero;
        //item.Rb.angularVelocity = Vector3.zero;


        // Set Slot as a parent
        item.transform.SetParent(slot);

        // Reset position and rotation
        item.transform.localPosition = Vector3.zero;
        item.transform.localEulerAngles = Vector3.zero;

    }

    /// <summary>
    /// Method for dropping item.
    /// </summary>
    /// <param name="item">Item.</param>
    private void DropItem(GameObject item)
    {
        Debug.Log("pickable: drop item");
        // Remove reference
        pickedItem = null;

        // Remove parent
        item.transform.SetParent(null);

        // Enable rigidbody
        item.GetComponent<Rigidbody>().isKinematic = false;

        // Add force to throw item a little bit
        item.GetComponent<Rigidbody>().AddForce(item.transform.forward * 2, ForceMode.VelocityChange);
    }
}



