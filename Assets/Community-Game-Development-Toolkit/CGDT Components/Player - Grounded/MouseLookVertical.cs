using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLookVertical : MonoBehaviour
{
    public float minimumVert = -45.0f;
    public float maximumVert = 45.0f;
    public float sensitivityVert = 9.0f;

    private float _rotationX = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _rotationX -= Input.GetAxis("Mouse Y") * sensitivityVert;
        _rotationX = Mathf.Clamp(_rotationX, minimumVert, maximumVert);

        float rotationY = transform.localEulerAngles.y;
        transform.localEulerAngles = new Vector3(_rotationX, rotationY, 0);
    }
}
