using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Click : MonoBehaviour
{
    private GameObject selected;
    private GameObject oldSelected;
    
    private Renderer selectionRenderer;
    private Renderer oldSelectionRenderer;

    private Camera _camera;

    private Color oldColor = Color.white;
    private Color newColor = Color.red;


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

        if (Input.GetMouseButtonDown(0)) //if clicked
        {
    
            Vector3 point = new Vector3(_camera.pixelWidth/2, _camera.pixelHeight/2, 0); //middle of camera view
            Ray ray = _camera.ScreenPointToRay(point);
            RaycastHit hit;
            

            if (Physics.Raycast(ray, out hit))
            {
                selected = hit.transform.gameObject;

                if (selected != oldSelected){

                    if (oldSelected != null) //returns previous selection to original color
                    {
                        oldSelectionRenderer = oldSelected.GetComponent<Renderer>();
                        oldSelectionRenderer.material.color = oldColor;
                    }

                    selectionRenderer = selected.GetComponent<Renderer>();
                    selectionRenderer.material.color = newColor; //sets a new color

                    oldSelected = selected;
                }
            }
            
        }
    }
}
