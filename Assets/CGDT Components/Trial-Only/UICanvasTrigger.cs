using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICanvasTrigger : MonoBehaviour
{
    public string triggerTag = "Player";
    public GameObject canvas;
    private bool isActive = false; 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("escape") || Input.GetKeyDown("return"))
        {
            if(isActive)
            {
                disable();
            }
        }  
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("trigger enter tag: " + openingTag);
        Debug.Log("UICanvas trigger enter: " + other.gameObject.tag);


        if (other.gameObject.tag == triggerTag)
        {
            Debug.Log("matched tag code: " + triggerTag);
            enable();
        }

    }

    private void OnTriggerExit(Collider other)
    {
        //Debug.Log("trigger enter tag: " + openingTag);
        Debug.Log("UICanvas trigger enter: " + other.gameObject.tag);


        if (other.gameObject.tag == triggerTag)
        {
            Debug.Log("matched tag code: " + triggerTag);
            disable();
        }

    }


    private void enable()
    {
        canvas.SetActive(true);
        isActive = true;
    }

    private void disable()
    {
        canvas.SetActive(false);
        isActive = false;

    }



}
