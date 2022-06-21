using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Click : MonoBehaviour
{
    private Color oldColor;
    private Color newColor = Color.red;

    private GameObject selected;
    private GameObject oldSelected;

    private Camera _camera;


    void Start()
    {
        _camera = GameObject.Find("Camera").GetComponent<Camera>();
        

        if (gameObject.GetComponent<Collider>() == null)
        //adds a box collider if an object doesn't have one
        {
            gameObject.AddComponent<BoxCollider>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        Renderer selectionRenderer;

        if (Input.GetMouseButtonDown(0)) //if clicked
        {
    
            Vector3 point = new Vector3(_camera.pixelWidth/2, _camera.pixelHeight/2, 0); //middle of camera view
            Ray ray = _camera.ScreenPointToRay(point);
            RaycastHit hit;
            

            if (Physics.Raycast(ray, out hit))
            {
                selected = hit.transform.gameObject;
                

                if (selected != oldSelected || oldSelected == null)
                {
                    selectionRenderer = selected.GetComponent<Renderer>();
                    oldColor = selectionRenderer.material.color; //saves the old color
                    selectionRenderer.material.color = newColor; //sets a new color
                    if (oldSelected != null)
                    {
                        oldSelected.GetComponent<Renderer>().material.color = oldColor;
                    }  
                }
            }
        }
    }
}
