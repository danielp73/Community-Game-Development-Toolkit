using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Click : MonoBehaviour
{
    private Color oldColor;
    private Color newColor = Color.red;


    void Start()
    {
        oldColor = GetComponent<Renderer>().material.color;
        if (gameObject.GetComponent<Collider>() == null)
        {
            gameObject.AddComponent<BoxCollider>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 point = new Vector3(Camera.main.pixelWidth/2, Camera.main.pixelHeight/2, 0);
            Ray ray = Camera.main.ScreenPointToRay(point);
            RaycastHit hit;
            

            if (Physics.Raycast(ray, out hit))
            {
                GameObject targetObject = hit.transform.gameObject;
                targetObject.GetComponent<Renderer>().material.color = newColor;
            
            }
        }
    }
}
