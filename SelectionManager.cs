using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    private string selectableTag = "Selectable";
    private Camera _camera;
    private Color oldColor;
    private Color newColor;

    private Transform _selection;

    void Start()
    {
        oldColor = gameObject.GetComponent<Renderer>().material.color;
        newColor = new Color32(255,255,255,55);
        _camera = GetComponent<Camera>();
    }
    // Update is called once per frame
    void Update()
    {
        if (_selection != null) //for returning the object back to it's
        //original color when deselected
        {
            _selection.GetComponent<Renderer>().material.color = oldColor;
            _selection = null;
        }
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 point = new Vector3(_camera.pixelWidth/2, _camera.pixelHeight/2, 0);
            Ray ray = _camera.ScreenPointToRay(point);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if(selection.CompareTag(selectableTag)) //where does selection come from?
                //check if it's selectable
                {
                    gameObject.GetComponent<Renderer>().material.color = newColor;
                }
                
                _selection = selection; //again where is selection?
            }
        }
    }
}
