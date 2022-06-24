using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Click : MonoBehaviour
{
    public GameObject selected;
    public GameObject oldSelected;
    
    private Renderer selectionRenderer;
    private Renderer oldSelectionRenderer;

    public Camera _camera;

    private Color oldColor = Color.white;
    private Color newColor = Color.red;


    void Start()
    {   //gets the camera attached to the player
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
        //cast a ray from the center of the camera's screen
        Vector3 point = new Vector3(_camera.pixelWidth/2, _camera.pixelHeight/2, 0); //middle of camera view
        Ray ray = _camera.ScreenPointToRay(point);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        { 

            if (Input.GetMouseButtonDown(0) && !selected) //if clicked
            {   
                selected = hit.transform.gameObject;

                if (selected != oldSelected) //if the object wasn't already selected
                {
                    if (oldSelected != null) //if there was a previous object
                    { //returns previous selection to original color
                        oldSelectionRenderer = oldSelected.GetComponent<Renderer>();
                        oldSelectionRenderer.material.color = oldColor;
                    }
                    //change the selected object to red
                    selectionRenderer = selected.GetComponent<Renderer>();
                    selectionRenderer.material.color = newColor; //sets a new color

                    oldSelected = selected; //set the newly red object as the previously selected
                }
            }
            else if (Input.GetMouseButton(0) && selected)
            {
                //GameObject Player = GetComponent<CharacterController>();

                if (Input.GetKey(KeyCode.J))
                {
                    transform.RotateAround(_camera.transform.position, Vector3.up, 20 * Time.deltaTime);
                }
            }

            
        }
    }
}
