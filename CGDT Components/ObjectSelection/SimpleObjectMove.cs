using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleObjectMove : MonoBehaviour
{

    public Click movableObject;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //update to run only if I am selected

        //combine with your Click script

        if(Input.GetKeyDown(KeyCode.J))
        {
            Vector3 updatedPosition = transform.position;
            updatedPosition.x -= 1;
            transform.position = updatedPosition;
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            Vector3 updatedPosition = transform.position;
            updatedPosition.x += 1;
            transform.position = updatedPosition;
        }
    }
}


