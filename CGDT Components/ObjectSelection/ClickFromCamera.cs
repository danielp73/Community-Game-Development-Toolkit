using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickFromCamera : MonoBehaviour
{
    public float rotateSpeed = 20f;

    private GameObject selected;
    private GameObject oldSelected;
    
    private Renderer selectionRenderer;
    private Renderer oldSelectionRenderer;

    private Camera _camera;

    private Color oldColor = Color.white;
    private Color newColor = Color.red;


    void Start()
    {   //gets the camera attached to the player
        _camera = GameObject.Find("Camera").GetComponent<Camera>();
        
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(0)) //if clicked
        {
            //cast a ray from the center of the camera's screen
            Vector3 point = new Vector3(_camera.pixelWidth / 2, _camera.pixelHeight / 2, 0); //middle of camera view
            Ray ray = _camera.ScreenPointToRay(point);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("raycast hit: " + hit.transform.gameObject.name);

                if (Input.GetMouseButtonDown(0)) //if clicked
                {
                    Debug.Log("clicked on: " + hit.transform.gameObject.name);

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
            }

        }

        //code to rotate selected object
        if (selected)
        {
            //rotate right
            if (Input.GetKey(KeyCode.L))
            {
                selected.transform.RotateAround(_camera.transform.position, Vector3.up, rotateSpeed * Time.deltaTime);
            }
            //rotate left
            if (Input.GetKey(KeyCode.J))
            {
                selected.transform.RotateAround(_camera.transform.position, Vector3.up, -rotateSpeed * Time.deltaTime);
            }
        }



    }
}
