using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Click : MonoBehaviour
{
    private GameObject selectedObject;
    private Renderer selectionRenderer;

    private Camera _camera;

    private Color oldColor;
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
                selectedObject = hit.collider.gameObject;
                if (selectedObject != null){
                    selectionRenderer = selectedObject.GetComponent<Renderer>();
                    oldColor = selectionRenderer.material.color; //saves the old color
                    selectionRenderer.material.color = newColor; //sets a new color
                }
            }
            
        }
    }
}
