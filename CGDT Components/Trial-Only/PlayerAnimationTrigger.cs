
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationTrigger : MonoBehaviour
{

    Animator animator;
    bool doorOpen;
    public string tagCode;

    private void Start()
    {
        doorOpen = false;
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("trigger enter tag: " + openingTag);
        Debug.Log("trigger enter: " + other.gameObject.tag);


        if (other.gameObject.tag == tagCode)
        {
            Debug.Log("trigger enter: " + other.gameObject.tag);
            Debug.Log("tag code: " + tagCode);
            doorOpen = true;
            SetTrigger("Open");
            Debug.Log("door opening");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("trigger exit");
        if (doorOpen)
        {
            if (other.gameObject.tag == tagCode)
            {
                doorOpen = false;
                SetTrigger("Close");
                Debug.Log("door closing");
            }
        }
    }

    void SetTrigger(string direction)
    {
        Debug.Log("setting trigger: " + direction);
        animator.SetTrigger(direction);
    }
}
