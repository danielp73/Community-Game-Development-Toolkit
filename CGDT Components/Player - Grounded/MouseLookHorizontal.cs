using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;



public class MouseLookHorizontal : MonoBehaviour
{
    private Camera _camera;
    private ToolkitInput input;

    public float sensitivityHor = 9.0f;

    // Start is called before the first frame update
    void Start()
    {

        _camera = GetComponent<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        input = GetComponent<ToolkitInput>();

    }

    // Update is called once per frame
    void Update()
    {
        if(input.Look.SqrMagnitude() > 0)
        {
            Debug.Log("Look: " + input.Look);
        }


        transform.Rotate(
            0,
            input.Look.x * sensitivityHor,
            0);
        }
}