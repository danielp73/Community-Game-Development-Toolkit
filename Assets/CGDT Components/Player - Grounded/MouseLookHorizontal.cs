using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class MouseLookHorizontal : MonoBehaviour
{
    private Camera _camera;

    public float sensitivityHor = 9.0f;

    // Start is called before the first frame update
    void Start()
    {

        _camera = GetComponent<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityHor, 0);
    }
}